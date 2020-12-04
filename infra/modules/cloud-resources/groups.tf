resource "azurerm_resource_group" "main" {
  name     = "rg-oriol-${var.service}-${var.env}"
  location = var.region
}
