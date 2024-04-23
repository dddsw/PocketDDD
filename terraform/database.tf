resource "azurerm_mssql_server" "sqlserver" {
  name                         = local.sql_server_name
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = random_string.admin_login.result
  administrator_login_password = random_password.admin_password.result

  tags = {
    environment = var.env
  }
}

resource "azurerm_mssql_database" "sqldb" {
  name                 = "${local.resource_prefix}-sqldatabase"
  server_id            = azurerm_mssql_server.sqlserver.id
  sku_name             = var.sql_db_sku
  max_size_gb          = var.sql_max_storage
  storage_account_type = "Local"

  tags = {
    environment = var.env
  }

  # prevent the possibility of accidental data loss
  lifecycle {
    prevent_destroy = false
  }
}

resource "azurerm_mssql_firewall_rule" "firewall_rule" {
  name             = "AllowAllAzureServices"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
