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
  name      = "${local.resource_prefix}-sqldatabase"
  server_id = azurerm_mssql_server.sqlserver.id

  tags = {
    environment = var.env
  }

  # prevent the possibility of accidental data loss
  lifecycle {
    prevent_destroy = false
  }
}
