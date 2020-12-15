# setup
terraform {
  backend "azurerm" {
    resource_group_name   = "rg-terraform-state"
    storage_account_name  = "oriolvarela" 
    container_name        = "platereader"
    key                   = "dev.terraform.tfstate"
    subscription_id       = "075254db-75bd-4d17-b2a8-9fa8cdff9f65" # Funda Dev/Test - Playground - https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade
    tenant_id             = "3a40f900-9165-459e-9aea-7eaf0d933d38" # Funda Real Estate BV - https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Overview
  }
}

locals {
  env           = "dev"
  region        = "WestEurope"
  region_short  = "we"
  service       = "platereader"
  service_short = "ptr"
  team          = "platform"
}
locals {
  tags = {
    env     = local.env
    team    = local.team
    service = local.service
  }
}

# cloud resources
module "cloud_resources" {
  source        = "../../modules/cloud-resources"
  providers     = {
    azurerm     = azurerm.funda_playground
  }
  env           = local.env
  service       = local.service
  service_short = local.service_short
  region        = local.region
}
