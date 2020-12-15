provider "azurerm" {
  features {}
  alias  = "funda_playground"
  subscription_id = "075254db-75bd-4d17-b2a8-9fa8cdff9f65" # Funda Dev/Test - Playground
}

provider "azurerm" {
  features {}
  alias  = "funda_dev"
  subscription_id = "69970448-9336-4a3e-b392-5207cce52637" # Funda Dev/Test - Development
}