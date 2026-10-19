output "site_url" {
  description = "Where the game is served."
  value       = "https://${var.domain_name != "" ? var.domain_name : "${local.hosting_site_id}.web.app"}"
}

output "hosting_site_id" {
  value = local.hosting_site_id
}

output "api_service" {
  value = google_cloud_run_v2_service.api.name
}

output "api_region" {
  value = google_cloud_run_v2_service.api.location
}

output "api_url" {
  description = "Direct URL of the Cloud Run service (the site reaches it through /api)."
  value       = google_cloud_run_v2_service.api.uri
}

output "custom_domain_dns_records" {
  description = "DNS records to create at your DNS provider for domain_name."
  value       = var.domain_name == "" ? null : google_firebase_hosting_custom_domain.site[0].required_dns_updates
}
