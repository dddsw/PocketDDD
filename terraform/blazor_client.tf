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

  preview_environments_enabled = false
}

resource "azurerm_key_vault_secret" "blazor_client_deployment_token" {
  name         = "${local.resource_prefix}-blazor-client-deployment-token"
  value        = azurerm_static_web_app.blazor-client.api_key
  key_vault_id = azurerm_key_vault.key_vault.id
}

data "cloudflare_zone" "dns_zone" {
  account_id = var.cloudflare_account_id
  name       = "dddsouthwest.com"
}

resource "cloudflare_record" "cname_record" {
  zone_id = data.cloudflare_zone.dns_zone.id
  name    = local.subdomain
  content = azurerm_static_web_app.blazor-client.default_host_name
  type    = "CNAME"
  ttl     = 3600
}

resource "azurerm_static_web_app_custom_domain" "custom_domain" {
  static_web_app_id = azurerm_static_web_app.blazor-client.id
  domain_name       = "${cloudflare_record.cname_record.name}.${data.cloudflare_zone.dns_zone.name}"
  validation_type   = "cname-delegation"
}
