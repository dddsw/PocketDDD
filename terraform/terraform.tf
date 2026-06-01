terraform {
  required_version = ">= 1.13.4"
  
  backend "azurerm" {
    resource_group_name  = "pocketddd-terraform-state"
    storage_account_name = "pocketdddterraformstate"
    container_name       = "tfstate"
  }
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.75.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.9.0"
    }
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 5.0"
    }
  }
}

provider "azurerm" {
  # Configuration options
  features {

  }
}
