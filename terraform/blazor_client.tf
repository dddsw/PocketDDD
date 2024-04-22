resource "azurerm_static_web_app" "blazor-client" {
  name                = "${local.resource_prefix}-blazorclient"
  location            = "westeurope"
  resource_group_name = azurerm_resource_group.rg.name

  sku_tier = var.client_sku_tier
  sku_size = var.client_sku_size
}

resource "github_actions_environment_secret" "test_secret" {
  repository       = data.github_repository.repo.name
  environment      = github_repository_environment.repo_environment.environment
  secret_name      = "AZURE_STATIC_WEB_APPS_API_TOKEN"
  plaintext_value  = azurerm_static_web_app.blazor-client.api_key
}