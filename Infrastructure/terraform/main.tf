provider "aws" {
  region                     = "us-east-1"
  access_key                 = "teste"
  secret_key                 = "teste"
  skip_credentials_validation = true
  skip_requesting_account_id = true
  skip_metadata_api_check    = true

  endpoints {
    apigateway = "http://localhost:4566"
    ssm        = "http://localhost:4566"
    dynamodb   = "http://localhost:4566"
    s3         = "http://localhost:4566"
    sqs        = "http://localhost:4566"
    sns        = "http://localhost:4566"
  }
}

