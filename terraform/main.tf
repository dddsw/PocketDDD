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

locals {
  db_connection_string = "Server=tcp:${local.sql_server_name}.database.windows.net,1433;Persist Security Info=False;User ID=${random_string.admin_login.result};Password=${random_password.admin_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}