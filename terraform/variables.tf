variable "project_id" {
  description = "Google Cloud project that hosts everything (the one prepared by bootstrap/)."
  type        = string
}

variable "region" {
  description = "Region of the Cloud Run service. Tier 1 regions (us-central1, us-east1, europe-west1, ...) are the cheapest."
  type        = string
  default     = "us-central1"
}

variable "project_name" {
  description = "Prefix used to name every resource. Must match project_name of the bootstrap stack."
  type        = string
  default     = "fortaleza"
}

variable "image" {
  description = "Container image of Fortaleza.Api, pinned by digest (pushed by deploy.ps1 or deploy.sh)."
  type        = string
}

variable "hosting_site_id" {
  description = "Firebase Hosting site created by bootstrap/. Empty means the project id."
  type        = string
  default     = ""
}

variable "memory" {
  description = "Memory of each Cloud Run instance."
  type        = string
  default     = "512Mi"
}

variable "request_timeout_seconds" {
  description = "Cloud Run request timeout. A request replays at most 1000 commands, which takes milliseconds."
  type        = number
  default     = 15
}

variable "max_instances" {
  description = "Caps Cloud Run instances to limit cost abuse. Each instance serves up to 80 requests at once."
  type        = number
  default     = 5
}

variable "domain_name" {
  description = "Optional custom domain (for example fortaleza.example.com). Firebase provisions the certificate; create the DNS records from the custom_domain_dns_records output."
  type        = string
  default     = ""
}
