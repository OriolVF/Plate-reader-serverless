using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.Azure.Core;
using Environment = System.Environment;

namespace TollBooth.Infra
{
    public class MyStack : Stack
    {
        public MyStack()
        {
            var stack = Environment.GetEnvironmentVariable("PULUMI_STACK");
            var resourceGroup = new ResourceGroup($"rg-oriol-tollbooth-{stack}");
            var resourceGroupName = resourceGroup.Name;

            var functionProvider = new FunctionApp("funcapps", resourceGroupName);
            var processImageFunction = functionProvider.AddFunction("ProcessImage", new Dictionary<string, string>()
            {
                { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
            });
            var manualReview = functionProvider.AddFunction("ManualReview", new Dictionary<string, string>()
            {
                { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
            });
            var savePlate = functionProvider.AddFunction("SavePlate",  new Dictionary<string, string>()
            {
                { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
            });

            var storageProvider = new StorageProvider(resourceGroupName, "plates");
            storageProvider.AddContainer("export");
            storageProvider.AddContainer("images");

            var cosmosDbProvider = new CosmosDbProvider(resourceGroupName, "plates-db");
            cosmosDbProvider.CreateSqlDb("LicensePlates");
            cosmosDbProvider.CreateContainer("Processed", "/licensePlateText", "Consistent");
            cosmosDbProvider.CreateContainer("NeedsManualReview", "/fileName", "Consistent");

            var cv = new ComputerVisionProvider("computer-vision-ocr", resourceGroupName);
            var systemTopic = new EventGridSystemTopicProvider(resourceGroupName, "eventgrid-system-topic",
                storageProvider.StorageAccount.Id);

            var eventgridTopic = new EventgGridTopic(resourceGroupName, "events-topic");
        }
    }
}
