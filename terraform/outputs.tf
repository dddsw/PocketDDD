output "api_server_url" {
  value = "https://${azurerm_linux_web_app.api_server_web_app.default_hostname}/"
}

output "client_app_public_url" {
  value = "https://${azurerm_static_web_app_custom_domain.custom_domain.domain_name}"
}
