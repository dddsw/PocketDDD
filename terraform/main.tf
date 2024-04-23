resource "azurerm_resource_group" "rg" {
  name     = "${local.resource_prefix}-rg"
  location = "UK South"
}

resource "random_string" "admin_login" {
  length           = 16
  special          = true
  override_special = "/@£$"
}

resource "random_password" "admin_password" {
  length = 25
}

resource "random_password" "admin_api_key" {
  length = 25
}

locals {
  db_connection_string = "Server=tcp:${local.sql_server_name}.database.windows.net,1433;Initial Catalog=pocketddd-dev-sqldatabase;Persist Security Info=False;User ID=${random_string.admin_login.result};Password=${random_password.admin_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}

data "github_repository" "repo" {
  name = "PocketDDD"
}

resource "github_repository_environment" "repo_environment" {
  repository  = "PocketDDD"
  environment = var.env
}
