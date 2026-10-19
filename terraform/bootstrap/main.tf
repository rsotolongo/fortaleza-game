data "google_project" "current" {}

locals {
  hosting_site_id = coalesce(var.hosting_site_id, var.project_id)

  services = [
    "artifactregistry.googleapis.com",
    "billingbudgets.googleapis.com",
    "cloudresourcemanager.googleapis.com",
    "firebase.googleapis.com",
    "firebasehosting.googleapis.com",
    "iam.googleapis.com",
    "iamcredentials.googleapis.com",
    "run.googleapis.com",
    "serviceusage.googleapis.com",
    "sts.googleapis.com",
  ]
}

resource "google_project_service" "enabled" {
  for_each = toset(local.services)

  service            = each.value
  disable_on_destroy = false
}

# ------------------------------------------------- Remote state for the main stack

resource "google_storage_bucket" "state" {
  name                        = "${var.project_id}-${var.project_name}-tfstate"
  location                    = var.region
  uniform_bucket_level_access = true
  public_access_prevention    = "enforced"

  versioning {
    enabled = true
  }

  lifecycle_rule {
    condition {
      num_newer_versions = 10
    }
    action {
      type = "Delete"
    }
  }

  # Losing this bucket means losing track of the deployed stack.
  lifecycle {
    prevent_destroy = true
  }
}

# --------------------------------------------------------- Images and hosting

resource "google_artifact_registry_repository" "images" {
  repository_id          = var.project_name
  location               = var.region
  format                 = "DOCKER"
  cleanup_policy_dry_run = false

  cleanup_policies {
    id     = "keep-recent"
    action = "KEEP"
    most_recent_versions {
      keep_count = 5
    }
  }

  cleanup_policies {
    id     = "delete-old"
    action = "DELETE"
    condition {
      older_than = "604800s"
    }
  }

  depends_on = [google_project_service.enabled]
}

resource "google_firebase_project" "current" {
  provider   = google-beta
  project    = var.project_id
  depends_on = [google_project_service.enabled]
}

resource "google_firebase_hosting_site" "site" {
  provider = google-beta
  count    = var.create_hosting_site ? 1 : 0

  project    = var.project_id
  site_id    = local.hosting_site_id
  depends_on = [google_firebase_project.current]
}

# The Cloud Run service runs as this account. It has no roles: the API calls no Google APIs.
resource "google_service_account" "api" {
  account_id   = "${var.project_name}-api"
  display_name = "Fortaleza API (Cloud Run runtime)"
  depends_on   = [google_project_service.enabled]
}

# ------------------------------------------------------- GitHub Actions identity

resource "google_iam_workload_identity_pool" "github" {
  workload_identity_pool_id = "${var.project_name}-github"
  display_name              = "GitHub Actions"
  depends_on                = [google_project_service.enabled]
}

resource "google_iam_workload_identity_pool_provider" "github" {
  workload_identity_pool_id          = google_iam_workload_identity_pool.github.workload_identity_pool_id
  workload_identity_pool_provider_id = "github"
  display_name                       = "GitHub OIDC"

  attribute_mapping = {
    "google.subject"          = "assertion.sub"
    "attribute.repository"    = "assertion.repository"
    "attribute.repository_id" = "assertion.repository_id"
    "attribute.ref"           = "assertion.ref"
  }
  # Matched on ids rather than the sub claim: its format changes when the repository opts into immutable
  # subjects, and a renamed or re-created repository with the same name must not be able to deploy.
  attribute_condition = "assertion.repository_id == '${var.github_repository_id}' && assertion.ref == 'refs/heads/${var.github_branch}'"

  oidc {
    issuer_uri = "https://token.actions.githubusercontent.com"
  }
}

resource "google_service_account" "deploy" {
  account_id   = "${var.project_name}-github-deploy"
  display_name = "Fortaleza GitHub deploy"
  depends_on   = [google_project_service.enabled]
}

resource "google_service_account_iam_member" "deploy_from_github" {
  service_account_id = google_service_account.deploy.name
  role               = "roles/iam.workloadIdentityUser"
  member             = "principalSet://iam.googleapis.com/${google_iam_workload_identity_pool.github.name}/attribute.repository_id/${var.github_repository_id}"
}

# The deploy account gets no IAM admin role anywhere, so CI cannot grant itself more permissions.

resource "google_storage_bucket_iam_member" "deploy_state" {
  bucket = google_storage_bucket.state.name
  role   = "roles/storage.objectAdmin"
  member = google_service_account.deploy.member
}

resource "google_artifact_registry_repository_iam_member" "deploy_push" {
  repository = google_artifact_registry_repository.images.name
  location   = google_artifact_registry_repository.images.location
  role       = "roles/artifactregistry.writer"
  member     = google_service_account.deploy.member
}

resource "google_service_account_iam_member" "deploy_acts_as_runtime" {
  service_account_id = google_service_account.api.name
  role               = "roles/iam.serviceAccountUser"
  member             = google_service_account.deploy.member
}

# run.admin (not run.developer) because making the service public needs run.services.setIamPolicy.
resource "google_project_iam_member" "deploy" {
  for_each = toset([
    "roles/run.admin",
    "roles/firebasehosting.admin",
    "roles/serviceusage.serviceUsageConsumer",
  ])

  project = var.project_id
  role    = each.value
  member  = google_service_account.deploy.member
}

# ------------------------------------------------------------------ Cost alert

resource "google_billing_budget" "monthly" {
  count = var.billing_account_id == "" ? 0 : 1

  billing_account = var.billing_account_id
  display_name    = "${var.project_name}-monthly"

  budget_filter {
    projects = ["projects/${data.google_project.current.number}"]
  }

  amount {
    specified_amount {
      currency_code = "USD"
      units         = tostring(var.budget_limit_usd)
    }
  }

  dynamic "threshold_rules" {
    for_each = [0.5, 0.8, 1.0]
    content {
      threshold_percent = threshold_rules.value
    }
  }

  depends_on = [google_project_service.enabled]
}
