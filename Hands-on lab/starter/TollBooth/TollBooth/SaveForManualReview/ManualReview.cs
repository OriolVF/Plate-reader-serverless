using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TollBooth.Models;

namespace TollBooth.SaveForManualReview
{
    public class ManualReview
    {
        [FunctionName("ManualReview")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (eventGridEvent?.Data != null)
                {
                    var endpointUri = Environment.GetEnvironmentVariable("cosmosDBEndPointUrl");
                    var key = Environment.GetEnvironmentVariable("cosmosDBAuthorizationKey");
                    var databaseName = Environment.GetEnvironmentVariable("cosmosDBDatabaseId");
                    var containerID = Environment.GetEnvironmentVariable("cosmosDBNeedsManualReviewId");
                    var licenseTextEvent = JsonConvert.DeserializeObject<LicensePlateData>(eventGridEvent.Data.ToString());

                    log.LogInformation($"Processing {licenseTextEvent.FileName} with plate {licenseTextEvent.LicensePlateText}");

                    using (var client = new CosmosClient(endpointUri, key))
                    {
                        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                        var db = databaseResponse.Database;
                        var containerOptions = new ContainerProperties(containerID, "/fileName")
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
                            licensePlateText = string.Empty,
                            timeStamp = licenseTextEvent.TimeStamp,
                            id = Guid.NewGuid()
                        });

                        if (response.StatusCode == HttpStatusCode.OK ||
                            response.StatusCode == HttpStatusCode.Accepted ||
                            response.StatusCode == HttpStatusCode.NoContent)
                        {
                            log.LogInformation($"Successfully isnerted row with {licenseTextEvent.FileName}");
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
