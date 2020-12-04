resource "azurerm_storage_account" "main" {
  name                     = "fda${var.service_short}${var.env}" # alpha-numeric only
  location                  = azurerm_resource_group.main.location
  resource_group_name       = azurerm_resource_group.main.name

  account_tier             = "Standard"
  account_replication_type = "LRS"
  allow_blob_public_access = true
}

resource "azurerm_storage_container" "images-cont" {
    name    = "images"
    container_access_type = "blob"
    storage_account_name = azurerm_storage_account.main.name
}