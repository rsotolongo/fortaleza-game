output "workload_identity_provider" {
  description = "Value for the GCP_WORKLOAD_IDENTITY_PROVIDER repository variable."
  value       = google_iam_workload_identity_pool_provider.github.name
}

output "deploy_service_account" {
  description = "Value for the GCP_DEPLOY_SERVICE_ACCOUNT repository variable."
  value       = google_service_account.deploy.email
}

output "state_bucket" {
  description = "Value for the TF_STATE_BUCKET repository variable."
  value       = google_storage_bucket.state.name
}

output "hosting_site_id" {
  description = "Set hosting_site_id in the main stack if this is not the project id."
  value       = local.hosting_site_id
}

output "github_variables_commands" {
  description = "Run these once to configure the repository."
  value       = <<-EOT
    gh variable set GCP_PROJECT_ID --repo ${var.github_repository} --body "${var.project_id}"
    gh variable set GCP_REGION --repo ${var.github_repository} --body "${var.region}"
    gh variable set GCP_WORKLOAD_IDENTITY_PROVIDER --repo ${var.github_repository} --body "${google_iam_workload_identity_pool_provider.github.name}"
    gh variable set GCP_DEPLOY_SERVICE_ACCOUNT --repo ${var.github_repository} --body "${google_service_account.deploy.email}"
    gh variable set TF_STATE_BUCKET --repo ${var.github_repository} --body "${google_storage_bucket.state.name}"
  EOT
}
