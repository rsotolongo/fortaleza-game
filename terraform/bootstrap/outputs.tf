output "deploy_role_arn" {
  description = "Value for the AWS_ROLE_ARN repository variable."
  value       = aws_iam_role.deploy.arn
}

output "state_bucket" {
  description = "Value for the TF_STATE_BUCKET repository variable."
  value       = aws_s3_bucket.state.id
}

output "aws_region" {
  description = "Value for the AWS_REGION repository variable."
  value       = var.aws_region
}

output "github_variables_commands" {
  description = "Run these once to configure the repository."
  value       = <<-EOT
    gh variable set AWS_ROLE_ARN --repo ${var.github_repository} --body "${aws_iam_role.deploy.arn}"
    gh variable set TF_STATE_BUCKET --repo ${var.github_repository} --body "${aws_s3_bucket.state.id}"
    gh variable set AWS_REGION --repo ${var.github_repository} --body "${var.aws_region}"
  EOT
}
