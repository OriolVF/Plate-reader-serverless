using System.Collections.Generic;
using System.IO;
using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.EventGrid;
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

            var appStorage = StorageAccountFactory.Create(resourceGroupName, new StorageAccountArgs()
            {
                Name = "plates",
                ReplicationType = "LRS",
                Tier = "Standard"
            });
            appStorage.CreateContainer("export");
            appStorage.CreateContainer("images");

            var db = CosmosDbFactory.CreateSqlDb("plates-db", resourceGroupName);
            db.CreateContainer("Processed", "/licensePlateText");
            db.CreateContainer("NeedsManualReview", "/fileName");


            var cv = ComputerVisionFactory.Create("cv-ocr", resourceGroupName);
            var topic = EventGridTopicFactory.Create(resourceGroupName, "events-topic");

            var functionApp = FunctionAppFactory.CreateAppPlan("functions-app", resourceGroupName, new FunctionAppPlanArgs()
            {
                Tier = "Dynamic",
                Size = "Y1"
            }).CreateFunction("functions", new Dictionary<string, Output<string>>()
            {
                { "FUNCTIONS_WORKER_RUNTIME", Output.Create("dotnet") },
                {"computerVisionApiUrl", cv.EndpointUrl },
                {"computerVisionApiKey", cv.ApiKey },
                {"eventGridTopicEndpoint", topic.Endpoint },
                {"eventGridTopicKey", topic.Key },
                {"cosmosDBEndPointUrl", db.PrimaryEndpoint },
                {"cosmosDBAuthorizationKey", db.AccessKey },
                {"cosmosDBDatabaseId", Output.Create("LicensePlates") },
                {"cosmosDBCollectionId", Output.Create("Processed") },
                {"cosmosDBNeedsManualReviewId", Output.Create("NeedsManualReview") },
                {"blobStorageConnection", appStorage.ConnectionString }

            }, new DeploymentArgs
            {
                PathToApp = Path.Combine("..", "TollBooth", "bin", "Debug", "netcoreapp3.1")
            });
        

            var systemTopic = EventGridSystemTopicFactory.Create(resourceGroupName, "sys-topic", appStorage.Id);

            var args = new Pulumi.Azure.EventGrid.Inputs.SystemTopicEventSubscriptionAzureFunctionEndpointArgs()
            {
                FunctionId = Output.Format($"{functionApp.Id}/functions/ProcessImage")
            };

            var SystemTopicSub = new SystemTopicEventSubscription("processImage", new SystemTopicEventSubscriptionArgs()
            {
                ResourceGroupName = resourceGroupName,
                SystemTopic = systemTopic.Name,
                AzureFunctionEndpoint = args
            });

            var topicSub = new EventSubscription("processedPlate", new EventSubscriptionArgs()
            {
                AzureFunctionEndpoint = new Pulumi.Azure.EventGrid.Inputs.EventSubscriptionAzureFunctionEndpointArgs()
                {
                    FunctionId = Output.Format($"{functionApp.Id}/functions/SavePlate"),
                },
                res
            })
        }
    }
}
