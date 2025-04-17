data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "rg" {
  name     = "${local.resource_prefix}-rg"
  location = "UK South"
}

resource "random_string" "admin_login" {
  length  = 20
  special = false
}

resource "random_password" "admin_password" {
  length  = 30
  special = false
}

resource "random_password" "admin_api_key" {
  length = 25
}

locals {
  db_connection_string = "Server=tcp:${local.sql_server_name}.database.windows.net,1433;Initial Catalog=pocketddd-${var.env}-sqldatabase;Persist Security Info=False;User ID=${random_string.admin_login.result};Password=${random_password.admin_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}

resource "azurerm_key_vault" "key_vault" {
  name                        = "${local.resource_prefix}-kv"
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get",
    ]

    secret_permissions = [
      "Get",
      "Set",
      "List",
      "Delete",
      "Purge",
      "Recover"
    ]

    storage_permissions = [
      "Get",
    ]
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = "4a9cec89-cee2-44fb-978f-6ded96b60d31"

    key_permissions = []

    secret_permissions = [
      "Get",
      "List",
      "Purge",
    ]

    storage_permissions = []
  }
}
