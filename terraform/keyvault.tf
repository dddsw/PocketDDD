data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "key_vault" {
  name                        = "${local.resource_prefix}-keyvault"
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
}

resource "azurerm_key_vault_secret" "sqldb_connectionstring" {
  name         = "${local.resource_prefix}-db-connection-string"
  value        = local.db_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_key_vault_secret" "sqldb_admin_password" {
  name         = "${local.resource_prefix}-db-admin-password"
  value        = random_password.admin_password.result
  key_vault_id = azurerm_key_vault.key_vault.id
}
