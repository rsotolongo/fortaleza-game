#!/usr/bin/env bash
# Builds the Vue site and the Lambda package, then applies the Terraform infrastructure.
#   ./deploy.sh                 deploy (asks for confirmation)
#   ./deploy.sh --auto-approve  deploy without confirmation
#   ./deploy.sh --destroy       tear everything down
set -euo pipefail

terraform_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root="$(dirname "$terraform_dir")"
build="$terraform_dir/build"
destroy=false
flags=()

for arg in "$@"; do
  case "$arg" in
    --destroy) destroy=true ;;
    --auto-approve) flags+=(-auto-approve) ;;
    *) echo "Unknown option: $arg" >&2; exit 1 ;;
  esac
done

if [ "$destroy" = false ]; then
  rm -rf "$build"

  echo "==> Install web dependencies"
  npm ci --prefix "$root/src/Fortaleza.Web"

  echo "==> Build the Vue site"
  npm run build --prefix "$root/src/Fortaleza.Web" -- --outDir "$build/site" --emptyOutDir

  echo "==> Publish the API for AWS Lambda (linux-arm64)"
  dotnet publish "$root/src/Fortaleza.Api" -c Release -r linux-arm64 --self-contained false \
    -p:PublishReadyToRun=true -p:ExcludeWwwroot=true -o "$build/publish"
fi

cd "$terraform_dir"
terraform init -input=false

if [ "$destroy" = true ]; then
  terraform destroy ${flags[@]+"${flags[@]}"}
else
  terraform apply ${flags[@]+"${flags[@]}"}
  terraform output site_url
fi
