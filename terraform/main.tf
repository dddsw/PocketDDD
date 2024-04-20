resource "azurerm_resource_group" "rg" {
  name     = "${local.resource_prefix}-rg"
  location = "UK South"
}

resource "random_string" "admin_login" {
  length           = 16
  special          = true
  override_special = "/@£$"
}

resource "random_password" "admin_password" {
  length = 25
}

locals {
  db_connection_string = "Server=tcp:${local.sql_server_name}.database.windows.net,1433;Persist Security Info=False;User ID=${random_string.admin_login.result};Password=${random_password.admin_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}

resource "azurerm_virtual_network" "vnet" {
  name                = "${local.resource_prefix}-vnet"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  address_space       = ["10.0.0.0/16"]
  dns_servers         = ["10.0.0.4", "10.0.0.5"]

  tags = {
    environment = var.env
  }
}

resource "azurerm_subnet" "subnet" {
  name                 = "default-subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "delegation"

    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action", "Microsoft.Network/virtualNetworks/subnets/prepareNetworkPolicies/action"]
    }
  }

  service_endpoints = ["Microsoft.Sql"]
}
