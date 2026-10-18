data "aws_caller_identity" "current" {}

locals {
  account_id        = data.aws_caller_identity.current.account_id
  github_oidc_url   = "https://token.actions.githubusercontent.com"
  lambda_role_arn   = "arn:aws:iam::${local.account_id}:role/${var.project_name}-api"
  oidc_provider_arn = var.create_github_oidc_provider ? aws_iam_openid_connect_provider.github[0].arn : "arn:aws:iam::${local.account_id}:oidc-provider/token.actions.githubusercontent.com"
}

# ------------------------------------------------- Remote state for the main stack

resource "aws_s3_bucket" "state" {
  bucket_prefix = "${var.project_name}-tfstate-"
}

resource "aws_s3_bucket_versioning" "state" {
  bucket = aws_s3_bucket.state.id

  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "state" {
  bucket = aws_s3_bucket.state.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

resource "aws_s3_bucket_public_access_block" "state" {
  bucket                  = aws_s3_bucket.state.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

# ------------------------------------------------------- GitHub Actions identity

resource "aws_iam_openid_connect_provider" "github" {
  count = var.create_github_oidc_provider ? 1 : 0

  url            = local.github_oidc_url
  client_id_list = ["sts.amazonaws.com"]
}

resource "aws_iam_role" "deploy" {
  name = "${var.project_name}-github-deploy"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect    = "Allow"
      Action    = "sts:AssumeRoleWithWebIdentity"
      Principal = { Federated = local.oidc_provider_arn }
      Condition = {
        StringEquals = {
          "token.actions.githubusercontent.com:aud" = "sts.amazonaws.com"
          "token.actions.githubusercontent.com:sub" = "repo:${var.github_repository}:ref:refs/heads/${var.github_branch}"
        }
      }
    }]
  })
}

data "aws_iam_policy_document" "deploy" {
  statement {
    sid       = "TerraformState"
    actions   = ["s3:ListBucket", "s3:GetObject", "s3:PutObject", "s3:DeleteObject"]
    resources = [aws_s3_bucket.state.arn, "${aws_s3_bucket.state.arn}/*"]
  }

  # Only the Lambda execution role of the main stack (named <project>-api) can be managed. The deploy role
  # itself is deliberately out of reach, otherwise CI could grant itself more permissions.
  statement {
    sid = "ManageLambdaRole"
    actions = [
      "iam:CreateRole", "iam:DeleteRole", "iam:GetRole", "iam:UpdateRole", "iam:UpdateAssumeRolePolicy",
      "iam:TagRole", "iam:UntagRole", "iam:ListRoleTags", "iam:ListAttachedRolePolicies",
      "iam:ListRolePolicies", "iam:ListInstanceProfilesForRole"
    ]
    resources = [local.lambda_role_arn]
  }

  statement {
    sid       = "AttachOnlyBasicLambdaLogging"
    actions   = ["iam:AttachRolePolicy", "iam:DetachRolePolicy"]
    resources = [local.lambda_role_arn]

    condition {
      test     = "ArnEquals"
      variable = "iam:PolicyARN"
      values   = ["arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"]
    }
  }

  statement {
    sid       = "PassLambdaRole"
    actions   = ["iam:PassRole"]
    resources = [local.lambda_role_arn]

    condition {
      test     = "StringEquals"
      variable = "iam:PassedToService"
      values   = ["lambda.amazonaws.com"]
    }
  }

  statement {
    sid       = "NeverTouchTheDeployRole"
    effect    = "Deny"
    actions   = ["iam:*"]
    resources = ["arn:aws:iam::${local.account_id}:role/${var.project_name}-github-deploy"]
  }

  statement {
    sid       = "ManageLambda"
    actions   = ["lambda:*"]
    resources = ["arn:aws:lambda:*:${local.account_id}:function:${var.project_name}-*"]
  }

  statement {
    sid = "ManageLogGroup"
    actions = [
      "logs:CreateLogGroup", "logs:DeleteLogGroup", "logs:PutRetentionPolicy", "logs:DeleteRetentionPolicy",
      "logs:TagResource", "logs:UntagResource", "logs:ListTagsForResource", "logs:ListTagsLogGroup"
    ]
    resources = ["arn:aws:logs:*:${local.account_id}:log-group:/aws/lambda/${var.project_name}-*", "arn:aws:logs:*:${local.account_id}:log-group:/aws/lambda/${var.project_name}-*:*"]
  }

  statement {
    sid       = "DescribeLogGroups"
    actions   = ["logs:DescribeLogGroups"]
    resources = ["*"]
  }

  statement {
    sid       = "ManageSiteBucket"
    actions   = ["s3:*"]
    resources = ["arn:aws:s3:::${var.project_name}-site-*", "arn:aws:s3:::${var.project_name}-site-*/*"]
  }

  statement {
    sid       = "ManageCloudFront"
    actions   = ["cloudfront:*"]
    resources = ["*"]
  }

  statement {
    sid       = "ManageBudgets"
    actions   = ["budgets:*"]
    resources = ["*"]
  }
}

resource "aws_iam_role_policy" "deploy" {
  name   = "deploy"
  role   = aws_iam_role.deploy.id
  policy = data.aws_iam_policy_document.deploy.json
}
