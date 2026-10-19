terraform {
  required_version = ">= 1.6"

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = ">= 8.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = ">= 8.0"
    }
  }
}

# The Budgets and Firebase APIs reject end-user credentials without a quota project.
provider "google" {
  project               = var.project_id
  region                = var.region
  user_project_override = true
  billing_project       = var.project_id

  default_labels = {
    project    = var.project_name
    managed-by = "terraform-bootstrap"
  }
}

provider "google-beta" {
  project               = var.project_id
  region                = var.region
  user_project_override = true
  billing_project       = var.project_id
}
