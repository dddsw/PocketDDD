terraform {
  backend "azurerm" {
    resource_group_name  = "pocketddd-terraform-state"
    storage_account_name = "pocketdddterraformstate"
    container_name       = "tfstate"
  }
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.100.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "3.6.1"
    }
  }
}

provider "azurerm" {
  # Configuration options
  features {

  }
}
