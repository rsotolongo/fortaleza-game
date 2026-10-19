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

provider "google" {
  project = var.project_id
  region  = var.region

  default_labels = {
    project    = var.project_name
    managed-by = "terraform"
  }
}

provider "google-beta" {
  project = var.project_id
  region  = var.region
}
