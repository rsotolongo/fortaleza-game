variable "aws_region" {
  description = "Region of the state bucket. Use the same region as the main stack."
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Must match project_name of the main stack: the deploy role may only manage resources named after it."
  type        = string
  default     = "fortaleza"
}

variable "github_repository" {
  description = "Repository allowed to deploy, as owner/name (for example rsotolongo/fortaleza-dotnet)."
  type        = string
}

variable "github_branch" {
  description = "Only workflows running on this branch can assume the deploy role."
  type        = string
  default     = "master"
}

variable "create_github_oidc_provider" {
  description = "Set to false if the AWS account already has the token.actions.githubusercontent.com OIDC provider (only one is allowed per account)."
  type        = bool
  default     = true
}
