resource "azurerm_eventgrid_topic" "eventstopic" {
  name                = "events-topic"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
}

resource "azurerm_servicebus_namespace" "namespace" {
  name                = "servicebus-namespace-platereader"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "Standard"

}

resource "azurerm_servicebus_queue" "processingqueue" {
  name                = "servicebus_queue"
  resource_group_name = azurerm_resource_group.main.name
  namespace_name      = azurerm_servicebus_namespace.namespace.name
}

resource "azurerm_eventgrid_event_subscription" "processImageUpload" {
  name  = "process-image"
  scope = azurerm_storage_account.main.id

  included_event_types = [
    "Microsoft.Storage.BlobCreated"
  ]
  service_bus_queue_endpoint_id = azurerm_servicebus_queue.processingqueue.id
}