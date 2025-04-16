locals {
  resource_prefix = "pocketddd-${var.env}"
  sql_server_name = "${local.resource_prefix}-sql-server"
  subdomain = var.env == "production" ? "pocket2025" : "pocket-${var.env}"
}