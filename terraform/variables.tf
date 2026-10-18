variable "aws_region" {
  description = "Region for the Lambda function, the S3 bucket and the logs."
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Prefix used to name every resource."
  type        = string
  default     = "fortaleza"
}

variable "publish_dir" {
  description = "Folder with the published Fortaleza.Api (created by deploy.ps1 or deploy.sh)."
  type        = string
  default     = "build/publish"
}

variable "site_dir" {
  description = "Folder with the built Vue site (created by deploy.ps1 or deploy.sh)."
  type        = string
  default     = "build/site"
}

variable "lambda_memory_mb" {
  description = "Lambda memory. 512 MB is plenty; more memory only buys a faster cold start."
  type        = number
  default     = 512
}

variable "lambda_timeout_seconds" {
  description = "Lambda timeout. A request replays at most 1000 commands, which takes milliseconds."
  type        = number
  default     = 15
}

variable "reserved_concurrency" {
  description = "Caps parallel Lambda executions to limit cost abuse. -1 means no cap. New AWS accounts often cannot reserve until their concurrency quota is raised."
  type        = number
  default     = -1
}

variable "log_retention_days" {
  description = "How long CloudWatch keeps the Lambda logs."
  type        = number
  default     = 14
}

variable "price_class" {
  description = "CloudFront price class. PriceClass_100 serves from North America and Europe and is the cheapest."
  type        = string
  default     = "PriceClass_100"
}

variable "domain_name" {
  description = "Optional custom domain (for example fortaleza.example.com). Requires certificate_arn and a DNS CNAME to the CloudFront domain."
  type        = string
  default     = ""
}

variable "certificate_arn" {
  description = "ARN of an ACM certificate for domain_name. It must be created in us-east-1."
  type        = string
  default     = ""
}

variable "budget_email" {
  description = "Optional email that receives an alert when the monthly cost passes 80% of budget_limit_usd."
  type        = string
  default     = ""
}

variable "budget_limit_usd" {
  description = "Monthly budget used by the optional cost alert."
  type        = number
  default     = 5
}
