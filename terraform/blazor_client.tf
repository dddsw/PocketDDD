resource "azurerm_static_web_app" "blazor-client" {
  name                = "${local.resource_prefix}-blazorclient"
  location            = "westeurope"
  resource_group_name = azurerm_resource_group.rg.name

  sku_tier = var.client_sku_tier
  sku_size = var.client_sku_size

  app_settings = {
    "apiUrl": "https://pocketddd-${ var.env }-api-server-web-app.azurewebsites.net/api/"
    "fakeBackend": "false"
  }
}

resource "azurerm_key_vault_secret" "blazor_client_deployment_token" {
  name         = "${local.resource_prefix}-blazor-client-deployment-token"
  value        = azurerm_static_web_app.blazor-client.api_key
  key_vault_id = azurerm_key_vault.key_vault.id
}
