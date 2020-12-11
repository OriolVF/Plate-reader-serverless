resource "azurerm_storage_account" "storagefunctions" {
  name                     = "storagefunctionsgr"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "functionserviceplan" {
  name                = "azure-functions-service-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  kind = "functionapp"
  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "processing" {
  name                       = "ProcessingFunctions"
  location                   = azurerm_resource_group.main.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_app_service_plan.functionserviceplan.id
  storage_account_name       = azurerm_storage_account.storagefunctions.name
  storage_account_access_key = azurerm_storage_account.storagefunctions.primary_access_key
  app_settings = {
        FUNCTIONS_WORKER_RUNTIME = "dotnet"
        "computerVisionApiUrl" = azurerm_cognitive_account.computervision.endpoint
        "computerVisionApiKey" = azurerm_cognitive_account.computervision.primary_access_key
        "eventGridTopicEndpoint" = azurerm_eventgrid_topic.eventstopic.endpoint
        "eventGridTopicKey" = azurerm_eventgrid_topic.eventstopic.primary_access_key
        "cosmosDBEndPointUrl" = azurerm_cosmosdb_account.db.endpoint
        "cosmosDBAuthorizationKey" = azurerm_cosmosdb_account.db.primary_key
        "cosmosDBDatabaseId" = "LicensePlates"
        "cosmosDBCollectionId" = "Processed"
        "cosmosDBNeedsManualReviewId" = "NeedsManualReview"
        "blobStorageConnection" = azurerm_storage_account.main.primary_connection_string
        "ServiceBusConnection" = azurerm_servicebus_namespace.namespace.default_primary_connection_string
    }
}

# In order to create events and subscriptions we need functions to be created. However there is no way of defining functions as resources,
# so we’ll use a dummy resource to ensure that the functions are always deployed.
# resource "null_resource" "functions" {

#   triggers = {
#     functions = "[processImage]"
#   }

#   provisioner "local-exec" {
#     command = "cd ..; func azure functionapp publish local.platereader; cd terraform"
#   }
# }