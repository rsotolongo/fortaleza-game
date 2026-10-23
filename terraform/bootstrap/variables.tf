variable "project_id" {
  description = "Existing Google Cloud project, linked to a billing account (Cloud Run needs the Blaze plan)."
  type        = string
}

variable "region" {
  description = "Region of the state bucket, the image repository and the Cloud Run service. Use the same one in the main stack."
  type        = string
  default     = "us-central1"
}

variable "project_name" {
  description = "Must match project_name of the main stack: the deploy identity may only use resources named after it."
  type        = string
  default     = "fortaleza"
}

variable "github_repository" {
  description = "Repository allowed to deploy, as owner/name (for example rsotolongo/fortaleza-game)."
  type        = string
}

variable "github_repository_id" {
  description = "Numeric id of github_repository: gh api repos/<owner>/<repo> --jq .id"
  type        = string
}

variable "github_branch" {
  description = "Only workflows running on this branch can impersonate the deploy service account."
  type        = string
  default     = "main"
}

variable "hosting_site_id" {
  description = "Firebase Hosting site to create; it becomes <id>.web.app. Empty means the project id (the default site)."
  type        = string
  default     = ""
}

variable "create_hosting_site" {
  description = "Set to false when the site already exists (for example the default site created from the Firebase console)."
  type        = bool
  default     = true
}

variable "billing_account_id" {
  description = "Optional billing account (XXXXXX-XXXXXX-XXXXXX) for a monthly budget alert; billing admins get the emails."
  type        = string
  default     = ""
}

variable "budget_limit_usd" {
  description = "Monthly budget used by the optional cost alert (alerts at 50%, 80% and 100%)."
  type        = number
  default     = 5
}
