locals {
  resource_prefix = "pocketddd-${var.env}"
  sql_server_name = "${local.resource_prefix}-sql-server"
  subdomain = var.env == "Production" ? "pocket" : "pocket-${var.env}"
}