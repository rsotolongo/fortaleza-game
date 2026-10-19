# Fortaleza on Google Cloud

Serverless and (almost) free at hobby traffic:

```
browser ──► Firebase Hosting (CDN) ──┬─ /*       ──► static files            Vue site
                                     └─ /api/**  ──► Cloud Run (.NET 10)     Fortaleza.Api
```

* The API keeps **no server state**. Every request carries the player's track (the commands so far),
  Cloud Run rebuilds the game by replaying it and runs the new command. Any instance can answer.
* Firebase Hosting serves the site over HTTPS from its CDN and forwards `/api/**` to Cloud Run on the same
  origin, so the browser needs no CORS.
* Firebase Hosting calls Cloud Run anonymously, so the service also answers on its own `run.app` URL. That
  exposes nothing the public `/api` path does not already expose; `max_instances` caps what abuse can cost.
* The container is built by the .NET SDK (`dotnet publish -t:PublishContainer`, no Dockerfile) on the
  chiseled Ubuntu ASP.NET image: no shell, non-root, listening on 8080.

## Layout

| Path | What it does |
|---|---|
| `bootstrap/` | Run once by an admin: enables the APIs, creates the state bucket, the Artifact Registry repository, the Firebase Hosting site, the runtime and deploy service accounts, GitHub Workload Identity Federation and an optional budget. |
| `main.tf` | The Cloud Run service, its public invoker binding and an optional Firebase custom domain. |
| `deploy.sh` / `deploy.ps1` | Build the site and the image, push the image, `terraform apply`, then `firebase deploy --only hosting`. |

The site files are published by the Firebase CLI, not Terraform: Terraform has no resource that uploads Hosting
files. The scripts generate `build/firebase.json` (the `/api/**` rewrite and cache headers) from the Terraform outputs.

## Prerequisites

* Terraform >= 1.6, .NET 10 SDK, Node 20+, Docker, and the [gcloud CLI](https://cloud.google.com/sdk/docs/install).
* A Google Cloud project **linked to a billing account** (Cloud Run requires it; on Firebase this is the Blaze
  plan, and the free quotas still apply).

## One-time setup

```bash
gcloud auth login
gcloud auth application-default login
cd terraform/bootstrap
cp terraform.tfvars.example terraform.tfvars   # set project_id and github_repository
terraform init
terraform apply
```

If the project already has its default Hosting site (it was opened in the Firebase console), set
`create_hosting_site = false`. To get a budget alert, set `billing_account_id` (your account needs
*Billing Account Administrator* or *Costs Manager* on it); the alert emails go to the billing account's admins.

## Deploy from your machine

```bash
export GOOGLE_CLOUD_PROJECT=my-fortaleza-project
export GOOGLE_CLOUD_REGION=us-central1
./terraform/deploy.sh             # Linux, macOS, Git Bash
```

```powershell
$env:GOOGLE_CLOUD_PROJECT = 'my-fortaleza-project'
./terraform/deploy.ps1            # PowerShell 7
```

The scripts log Docker in to Artifact Registry with `gcloud auth print-access-token`, so `gcloud auth configure-docker`
is not needed (snap's confined Docker cannot run that credential helper anyway). Add `--auto-approve`
(`-AutoApprove`) to skip the confirmation. The last line printed is the site URL
(`https://<site>.web.app`). Re-running it deploys updates: a new image is pushed and pinned by digest, and Firebase
uploads only the changed files. `index.html` is served with `no-cache` and the hashed `assets/` as immutable.

Tear everything down with `./terraform/deploy.sh --destroy` (or `-Destroy`). The bootstrap resources stay.

## Options

Copy `terraform.tfvars.example` to `terraform.tfvars`:

| Variable | Purpose |
|---|---|
| `max_instances` | Caps Cloud Run instances (cost guard), 5 by default. Unlike Lambda reserved concurrency it needs no quota increase. |
| `memory` | `512Mi` by default. |
| `domain_name` | Custom domain served by Firebase Hosting; see below. |
| `hosting_site_id` | When bootstrap created a site other than the project id. |

## Automatic deployment (GitHub Actions)

`.github/workflows/deploy.yml` runs on every push or merge to `master` in three jobs: **test**, then **build**
(npm and NuGet run here, with no Google credentials at all; the site and the container image tarball are uploaded
as an artifact), then **deploy**, the only job that can impersonate the deploy service account. It pushes the
pre-built image, runs Terraform and publishes the site (`deploy.sh --skip-build`). The Firebase CLI is installed
with `--ignore-scripts`, so no package install script runs next to the credentials.

Authentication uses **Workload Identity Federation**, so no service account keys exist: the provider only accepts
tokens whose subject is `repo:<owner>/<repo>:ref:refs/heads/master`. The Terraform state lives in a versioned GCS
bucket; the `gcs` backend locks natively.

After the bootstrap, configure the repository with the `github_variables_commands` output:

```bash
gh variable set GCP_PROJECT_ID                 --repo <owner>/<repo> --body "<project id>"
gh variable set GCP_REGION                     --repo <owner>/<repo> --body "<region>"
gh variable set GCP_WORKLOAD_IDENTITY_PROVIDER --repo <owner>/<repo> --body "<workload_identity_provider>"
gh variable set GCP_DEPLOY_SERVICE_ACCOUNT     --repo <owner>/<repo> --body "<deploy_service_account>"
gh variable set TF_STATE_BUCKET                --repo <owner>/<repo> --body "<state_bucket>"
```

Push to `master` (or run the **Deploy** workflow manually) and the site URL appears in the job summary.

Notes:

* The deploy account holds `run.admin`, `firebasehosting.admin` and `serviceUsageConsumer` on the project, object
  access to the state bucket only, push access to the image repository only, and may act only as the runtime service
  account. It has no IAM or service-account admin role, so CI cannot grant itself more permissions. If an apply ends
  in `PERMISSION_DENIED`, extend `google_project_iam_member.deploy` in `bootstrap/main.tf` and re-apply it locally.
* Organizations with the *Domain restricted sharing* policy block `allUsers`; the project then needs an exception,
  or Firebase Hosting cannot reach Cloud Run.
* Workflow actions are pinned to full commit SHAs; Dependabot (`.github/dependabot.yml`) opens weekly PRs to update them.
* **Pick one place to deploy from.** CI uses the GCS state through a generated `backend_override.tf`
  (git-ignored); running the scripts locally uses local state, which does not know what CI created. To deploy
  by hand against the same stack, copy that override file locally and run `terraform init -migrate-state`
  first. Keep the bootstrap `terraform.tfstate` safe (or re-import) since it is local.
* Terraform variables for CI have no `terraform.tfvars` on the runner: set them as `TF_VAR_*` environment variables
  in `deploy.yml`, for example `TF_VAR_domain_name`.
* For a manual approval before production, add `environment: production` to the `deploy` job and change
  `github_subject` in `bootstrap/main.tf` to `repo:<owner>/<repo>:environment:production`.

## Custom domain

1. Set `domain_name = "fortaleza.example.com"` (or `TF_VAR_domain_name` in CI) and deploy.
2. Create the DNS records listed in the `custom_domain_dns_records` output at your DNS provider.
3. Firebase verifies the domain and provisions the certificate itself, usually within an hour. `site_url` already
   shows the custom domain.

## Cost

At hobby traffic this costs about $0:

* **Cloud Run** (request-based billing, scales to zero): 2M requests, 180,000 vCPU-seconds and 360,000 GiB-seconds
  free per month. A game command takes milliseconds. Beyond that, roughly $0.40 per million requests plus compute.
* **Firebase Hosting**: 10 GB stored and 10 GB transferred free per month, then about $0.15 per GB served.
  This is the free tier most likely to be exceeded first.
* **Artifact Registry**: 0.5 GB free; the cleanup policy keeps the 5 newest images (about 100 MB each).
* **Cloud Logging**: 50 GB ingestion free per project, 30-day retention.

## Notes

* Firebase Hosting rewrites to Cloud Run time out after 60 seconds; the API answers in milliseconds.
* The image is built for `linux-x64` (Cloud Run runs x86-64) with `PublishReadyToRun`, which shortens the cold start.
* Local development is unchanged: `dotnet run --project src/Fortaleza.Api` (serves `wwwroot` when the
  site has been built) or `npm run dev` in `src/Fortaleza.Web`, which proxies `/api` to port 5080.
