#!/usr/bin/env bash
# Builds the Vue site and the API container, pushes the image, applies Terraform and publishes the site.
#   ./deploy.sh                 deploy (asks for confirmation)
#   ./deploy.sh --auto-approve  deploy without confirmation
#   ./deploy.sh --destroy       tear everything down
#   ./deploy.sh --skip-build    reuse build/site and build/api-image.tar.gz (CI builds them in a separate job)
#
# Needs GOOGLE_CLOUD_PROJECT (and optionally GOOGLE_CLOUD_REGION, default us-central1), Docker logged in to
# Artifact Registry, and Google credentials for Terraform and the Firebase CLI (gcloud auth application-default login).
set -euo pipefail

terraform_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root="$(dirname "$terraform_dir")"
build="$terraform_dir/build"
destroy=false
skip_build=false
flags=()

for arg in "$@"; do
  case "$arg" in
    --destroy) destroy=true ;;
    --skip-build) skip_build=true ;;
    --auto-approve) flags+=(-auto-approve) ;;
    *) echo "Unknown option: $arg" >&2; exit 1 ;;
  esac
done

project="${GOOGLE_CLOUD_PROJECT:?Set GOOGLE_CLOUD_PROJECT to the Google Cloud project id}"
region="${GOOGLE_CLOUD_REGION:-us-central1}"
project_name="${TF_VAR_project_name:-fortaleza}"
# FIREBASE_CLI lets CI use a copy installed without npm install scripts.
read -r -a firebase <<< "${FIREBASE_CLI:-npx --yes firebase-tools@15.32.1}"
export TF_VAR_project_id="$project" TF_VAR_region="$region"

cd "$terraform_dir"

if [ "$destroy" = true ]; then
  terraform init -input=false
  "${firebase[@]}" hosting:disable --project "$project" --site "$(terraform output -raw hosting_site_id)" --force || true
  terraform destroy -var image=unused ${flags[@]+"${flags[@]}"}
  exit 0
fi

if [ "$skip_build" = false ]; then
  rm -rf "$build"

  echo "==> Install web dependencies"
  npm ci --prefix "$root/src/Fortaleza.Web"

  echo "==> Build the Vue site"
  npm run build --prefix "$root/src/Fortaleza.Web" -- --outDir "$build/site" --emptyOutDir

  echo "==> Build the API container (linux-x64)"
  dotnet publish "$root/src/Fortaleza.Api" -c Release -r linux-x64 -t:PublishContainer \
    -p:PublishReadyToRun=true -p:ExcludeWwwroot=true -p:ContainerFamily=noble-chiseled \
    -p:ContainerRepository=fortaleza-api -p:ContainerImageTag=build \
    -p:ContainerArchiveOutputPath="$build/api-image.tar.gz"
fi

echo "==> Push the API image"
registry="$region-docker.pkg.dev"
image="$registry/$project/$project_name/api:$(date -u +%Y%m%d%H%M%S)"
# A token login instead of `gcloud auth configure-docker`: snap's confined Docker cannot run the gcloud
# credential helper. CI logs in with its own token.
if [ -z "${CI:-}" ]; then
  gcloud auth print-access-token | docker login -u oauth2accesstoken --password-stdin "https://$registry"
fi
docker load --input "$build/api-image.tar.gz"
docker tag fortaleza-api:build "$image"
docker push "$image"
# The loaded image also carries a digest for its local name, so pick the one in Artifact Registry.
digest="$(docker inspect --format '{{range .RepoDigests}}{{println .}}{{end}}' "$image" | grep -m1 "^${image%:*}@sha256:" || true)"
[ -n "$digest" ] || { echo "No Artifact Registry digest found for $image" >&2; exit 1; }

terraform init -input=false
terraform apply -var "image=$digest" ${flags[@]+"${flags[@]}"}

echo "==> Publish the site to Firebase Hosting"
cat > "$build/firebase.json" <<JSON
{
  "hosting": {
    "site": "$(terraform output -raw hosting_site_id)",
    "public": "site",
    "rewrites": [
      { "source": "/api/**", "run": { "serviceId": "$(terraform output -raw api_service)", "region": "$(terraform output -raw api_region)" } }
    ],
    "headers": [
      { "source": "{/,/index.html}", "headers": [{ "key": "Cache-Control", "value": "no-cache" }] },
      { "source": "/assets/**", "headers": [{ "key": "Cache-Control", "value": "public, max-age=31536000, immutable" }] }
    ]
  }
}
JSON
(cd "$build" && "${firebase[@]}" deploy --only hosting --project "$project" --non-interactive)

terraform output site_url
