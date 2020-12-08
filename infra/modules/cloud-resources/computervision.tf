resource "azurerm_cognitive_account" "computervision" {
  name                = "computer-vision-ocr"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  kind                = "ComputerVision"
  sku_name = "S1"
}