resource "azurerm_eventgrid_system_topic" "eventgridsystemtopic" {
  name                   = "eventgrid-system-topic"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  source_arm_resource_id = azurerm_storage_account.main.id
  topic_type             = "Microsoft.Storage.StorageAccounts"
}

resource "azurerm_eventgrid_topic" "eventstopic" {
  name                = "events-topic"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
}

# resource "azurerm_eventgrid_event_subscription" "processImageUpload" {
#   name  = "process-image"
#   scope = azurerm_resource_group.main.id

#   included_event_types = [
#     "Microsoft.Storage.BlobCreated"
#   ]

#   azure_function_endpoint {
#       function_id = "${azurerm_function_app.processing.id}/functions/processImage"
#   }
# }
