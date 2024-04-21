resource "azurerm_service_plan" "api_server_service_plan" {
  name                = "${local.resource_prefix}-api-server-serviceplan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "P0v3"
}


resource "azurerm_linux_web_app" "api_server_web_app" {
  name                = "${local.resource_prefix}-api-server"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.api_server_service_plan.location
  service_plan_id     = azurerm_service_plan.api_server_service_plan.id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
  }

  connection_string {
    name  = "PocketDDDContext"
    type  = "SQLAzure"
    value = local.db_connection_string
  }
}
