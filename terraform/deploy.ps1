#requires -Version 7
<#
.SYNOPSIS
  Builds the Vue site and the API container, pushes the image, applies Terraform and publishes the site.
.DESCRIPTION
  Needs $env:GOOGLE_CLOUD_PROJECT (and optionally $env:GOOGLE_CLOUD_REGION, default us-central1), Docker logged in
  to Artifact Registry, and Google credentials for Terraform and the Firebase CLI
  (gcloud auth application-default login).
.PARAMETER AutoApprove
  Skip the interactive confirmation of terraform apply.
.PARAMETER Destroy
  Tear the whole stack down instead of deploying.
.PARAMETER SkipBuild
  Reuse build/site and build/api-image.tar.gz instead of rebuilding them.
#>
param(
  [switch]$AutoApprove,
  [switch]$Destroy,
  [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$terraformDir = $PSScriptRoot
$root = Split-Path $terraformDir -Parent
$build = Join-Path $terraformDir 'build'
$site = Join-Path $build 'site'
$imageArchive = Join-Path $build 'api-image.tar.gz'

$project = $env:GOOGLE_CLOUD_PROJECT
if (-not $project) { throw 'Set $env:GOOGLE_CLOUD_PROJECT to the Google Cloud project id' }
$region = if ($env:GOOGLE_CLOUD_REGION) { $env:GOOGLE_CLOUD_REGION } else { 'us-central1' }
$projectName = if ($env:TF_VAR_project_name) { $env:TF_VAR_project_name } else { 'fortaleza' }
$firebase = if ($env:FIREBASE_CLI) { -split $env:FIREBASE_CLI } else { @('npx', '--yes', 'firebase-tools@15.32.1') }
$env:TF_VAR_project_id = $project
$env:TF_VAR_region = $region
$flags = if ($AutoApprove) { @('-auto-approve') } else { @() }

function Invoke-Step([string]$Title, [scriptblock]$Action) {
  Write-Host "==> $Title" -ForegroundColor Cyan
  & $Action
  if ($LASTEXITCODE -ne 0) { throw "$Title failed (exit code $LASTEXITCODE)" }
}

function Invoke-Firebase {
  & $firebase[0] @($firebase | Select-Object -Skip 1) @args
}

Push-Location $terraformDir
try {
  if ($Destroy) {
    Invoke-Step 'terraform init' { terraform init -input=false }
    Invoke-Firebase hosting:disable --project $project --site (terraform output -raw hosting_site_id) --force
    Invoke-Step 'terraform destroy' { terraform destroy -var image=unused @flags }
    return
  }

  if (-not $SkipBuild) {
    if (Test-Path $build) { Remove-Item $build -Recurse -Force }

    Invoke-Step 'Install web dependencies' { npm ci --prefix (Join-Path $root 'src/Fortaleza.Web') }
    Invoke-Step 'Build the Vue site' {
      npm run build --prefix (Join-Path $root 'src/Fortaleza.Web') -- --outDir $site --emptyOutDir
    }
    Invoke-Step 'Build the API container (linux-x64)' {
      dotnet publish (Join-Path $root 'src/Fortaleza.Api') -c Release -r linux-x64 -t:PublishContainer `
        -p:PublishReadyToRun=true -p:ExcludeWwwroot=true -p:ContainerFamily=noble-chiseled `
        -p:ContainerRepository=fortaleza-api -p:ContainerImageTag=build `
        "-p:ContainerArchiveOutputPath=$imageArchive"
    }
  }

  $registry = "$region-docker.pkg.dev"
  $image = "$registry/$project/$projectName/api:$((Get-Date).ToUniversalTime().ToString('yyyyMMddHHmmss'))"
  # A token login instead of `gcloud auth configure-docker`: snap's confined Docker cannot run the gcloud
  # credential helper. CI logs in with its own token.
  if (-not $env:CI) {
    Invoke-Step 'Log in to Artifact Registry' {
      gcloud auth print-access-token | docker login -u oauth2accesstoken --password-stdin "https://$registry"
    }
  }
  Invoke-Step 'Load the API image' { docker load --input $imageArchive }
  Invoke-Step 'Tag the API image' { docker tag fortaleza-api:build $image }
  Invoke-Step 'Push the API image' { docker push $image }
  # The loaded image also carries a digest for its local name, so pick the one in Artifact Registry.
  $repository = $image.Substring(0, $image.LastIndexOf(':'))
  $digest = docker inspect --format '{{range .RepoDigests}}{{println .}}{{end}}' $image |
    Where-Object { $_.StartsWith("$repository@sha256:") } | Select-Object -First 1
  if (-not $digest) { throw "No Artifact Registry digest found for $image" }

  Invoke-Step 'terraform init' { terraform init -input=false }
  Invoke-Step 'terraform apply' { terraform apply -var "image=$digest" @flags }

  $firebaseConfig = [ordered]@{
    hosting = [ordered]@{
      site     = terraform output -raw hosting_site_id
      public   = 'site'
      rewrites = @(@{ source = '/api/**'; run = @{ serviceId = (terraform output -raw api_service); region = (terraform output -raw api_region) } })
      headers  = @(
        @{ source = '{/,/index.html}'; headers = @(@{ key = 'Cache-Control'; value = 'no-cache' }) },
        @{ source = '/assets/**'; headers = @(@{ key = 'Cache-Control'; value = 'public, max-age=31536000, immutable' }) }
      )
    }
  }
  $firebaseConfig | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $build 'firebase.json')

  Push-Location $build
  try {
    Invoke-Step 'Publish the site to Firebase Hosting' { Invoke-Firebase deploy --only hosting --project $project --non-interactive }
  }
  finally {
    Pop-Location
  }

  terraform output site_url
}
finally {
  Pop-Location
}
