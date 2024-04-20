output "api_server_url" {
  value = "https://${azurerm_linux_web_app.api_server_web_app.default_hostname}/"
}