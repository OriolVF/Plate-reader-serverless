// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

// Learn how to locally debug an Event Grid-triggered function:
//    https://aka.ms/AA30pjh

// Use for local testing:
//   https://{ID}.ngrok.io/runtime/webhooks/EventGrid?functionName=ProcessImage

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TollBooth.Models;

namespace TollBooth
{
    public class SavePlate
    {
        [FunctionName("SavePlate")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (eventGridEvent?.Data != null)
                {
                    var endpointUri = Environment.GetEnvironmentVariable("cosmosDBEndPointUrl");
                    var key = Environment.GetEnvironmentVariable("cosmosDBAuthorizationKey");
                    var databaseName = Environment.GetEnvironmentVariable("cosmosDBDatabaseId");
                    var containerID = Environment.GetEnvironmentVariable("cosmosDBCollectionId");
                    var licenseTextEvent = JsonConvert.DeserializeObject<LicensePlateData>(eventGridEvent.Data.ToString());

                    log.LogInformation($"Processing {licenseTextEvent.FileName} with plate {licenseTextEvent.LicensePlateText}");

                    using (CosmosClient client = new CosmosClient(endpointUri, key))
                    {
                        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                        var db = databaseResponse.Database;
                        var containerOptions = new ContainerProperties(containerID, "/licensePlateText")
                        {
                            IndexingPolicy = new IndexingPolicy()
                            {
                                Automatic = true
                            }
                        };
                        var containerResponse = await db.CreateContainerIfNotExistsAsync(containerOptions);

                        var container = containerResponse.Container;
                        var response = await container.CreateItemAsync(new
                        {
                            fileName = licenseTextEvent.FileName,
                            licensePlateText = licenseTextEvent.LicensePlateText,
                            timeStamp = licenseTextEvent.TimeStamp,
                            id = Guid.NewGuid()
                        });

                        if (response.StatusCode == HttpStatusCode.OK ||
                            response.StatusCode == HttpStatusCode.Accepted ||
                            response.StatusCode == HttpStatusCode.NoContent)
                        {
                            log.LogInformation($"Successfully isnerted row with {licenseTextEvent.LicensePlateText} {licenseTextEvent.FileName}");
                        }
                        else
                        {
                            log.LogError($"Failed inserting row with {licenseTextEvent.LicensePlateText} {licenseTextEvent.FileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex.Message);
                throw;
            }
        }
    }
}
