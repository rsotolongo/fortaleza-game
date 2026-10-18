output "site_url" {
  description = "Where the game is served."
  value       = "https://${local.custom_domain ? var.domain_name : aws_cloudfront_distribution.site.domain_name}"
}

output "cloudfront_domain_name" {
  description = "Point the CNAME of a custom domain here."
  value       = aws_cloudfront_distribution.site.domain_name
}

output "cloudfront_distribution_id" {
  value = aws_cloudfront_distribution.site.id
}

output "lambda_function_name" {
  value = aws_lambda_function.api.function_name
}

output "site_bucket" {
  value = aws_s3_bucket.site.id
}
