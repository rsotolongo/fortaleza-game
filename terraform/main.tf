locals {
  name            = var.project_name
  hosting_site_id = coalesce(var.hosting_site_id, var.project_id)
  # Created by bootstrap/, so the deploy identity only needs to act as it, never to manage service accounts.
  runtime_service_account = "${local.name}-api@${var.project_id}.iam.gserviceaccount.com"
}

# ------------------------------------------------------------ API (Cloud Run)

resource "google_cloud_run_v2_service" "api" {
  name                = "${local.name}-api"
  location            = var.region
  ingress             = "INGRESS_TRAFFIC_ALL"
  deletion_protection = false

  template {
    service_account                  = local.runtime_service_account
    timeout                          = "${var.request_timeout_seconds}s"
    max_instance_request_concurrency = 80

    scaling {
      min_instance_count = 0
      max_instance_count = var.max_instances
    }

    containers {
      image = var.image

      ports {
        container_port = 8080
      }

      resources {
        limits = {
          cpu    = "1"
          memory = var.memory
        }
        cpu_idle          = true
        startup_cpu_boost = true
      }

      startup_probe {
        tcp_socket {
          port = 8080
        }
      }
    }
  }
}

# Firebase Hosting rewrites call the service anonymously, so it must be public. The API holds no state and no
# secrets, and reaching it on its run.app URL gives nothing the /api path of the site does not already give.
resource "google_cloud_run_v2_service_iam_member" "public" {
  name     = google_cloud_run_v2_service.api.name
  location = google_cloud_run_v2_service.api.location
  role     = "roles/run.invoker"
  member   = "allUsers"
}

# ---------------------------------------------------- Site (Firebase Hosting)

# The site files and the /api/** rewrite are published by the deploy script with the Firebase CLI.
resource "google_firebase_hosting_custom_domain" "site" {
  provider = google-beta
  count    = var.domain_name == "" ? 0 : 1

  project               = var.project_id
  site_id               = local.hosting_site_id
  custom_domain         = var.domain_name
  wait_dns_verification = false
}
