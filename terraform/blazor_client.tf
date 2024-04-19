resource "azurerm_static_web_app" "blazor-client" {
  name                = "${local.resource_prefix}-blazorclient"
  location            = "westeurope"
  resource_group_name = azurerm_resource_group.rg.name
}