#requires -Version 7
<#
.SYNOPSIS
  Builds the Vue site and the Lambda package, then applies the Terraform infrastructure.
.PARAMETER AutoApprove
  Skip the interactive confirmation of terraform apply.
.PARAMETER Destroy
  Tear the whole stack down instead of deploying.
#>
param(
  [switch]$AutoApprove,
  [switch]$Destroy
)

$ErrorActionPreference = 'Stop'
$terraformDir = $PSScriptRoot
$root = Split-Path $terraformDir -Parent
$build = Join-Path $terraformDir 'build'
$site = Join-Path $build 'site'
$publish = Join-Path $build 'publish'

function Invoke-Step([string]$Title, [scriptblock]$Action) {
  Write-Host "==> $Title" -ForegroundColor Cyan
  & $Action
  if ($LASTEXITCODE -ne 0) { throw "$Title failed (exit code $LASTEXITCODE)" }
}

if (-not $Destroy) {
  if (Test-Path $build) { Remove-Item $build -Recurse -Force }

  Invoke-Step 'Install web dependencies' { npm ci --prefix (Join-Path $root 'src/Fortaleza.Web') }
  Invoke-Step 'Build the Vue site' {
    npm run build --prefix (Join-Path $root 'src/Fortaleza.Web') -- --outDir $site --emptyOutDir
  }
  Invoke-Step 'Publish the API for AWS Lambda (linux-arm64)' {
    dotnet publish (Join-Path $root 'src/Fortaleza.Api') -c Release -r linux-arm64 --self-contained false `
      -p:PublishReadyToRun=true -p:ExcludeWwwroot=true -o $publish
  }
}

Push-Location $terraformDir
try {
  Invoke-Step 'terraform init' { terraform init -input=false }
  $mode = if ($Destroy) { 'destroy' } else { 'apply' }
  $flags = if ($AutoApprove) { @('-auto-approve') } else { @() }
  Invoke-Step "terraform $mode" { terraform $mode @flags }
  if (-not $Destroy) { terraform output site_url }
}
finally {
  Pop-Location
}
