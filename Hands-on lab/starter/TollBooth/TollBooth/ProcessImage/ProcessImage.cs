// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

// Learn how to locally debug an Event Grid-triggered function:
//    https://aka.ms/AA30pjh

// Use for local testing:
//   https://{ID}.ngrok.io/runtime/webhooks/EventGrid?functionName=ProcessImage

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using TollBooth.Models;

namespace TollBooth
{
    public class ProcessImage
    {
        private readonly ComputerVisionClient _cvClient;
        private readonly EventGridPublisher _eventGridPublisher;
        public ProcessImage(ComputerVisionClient cvClient, 
            EventGridPublisher eventGridPublisher)
        {
            _cvClient = cvClient;
            _eventGridPublisher = eventGridPublisher;
        }
        private static string GetBlobNameFromUrl(string bloblUrl)
        {
            var uri = new Uri(bloblUrl);
            var cloudBlob = new CloudBlob(uri);
            return cloudBlob.Name;
        }

        [FunctionName("ProcessImage")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent,
                                     [Blob(blobPath: "{data.url}", 
                                         access: FileAccess.Read,
                                         Connection = "blobStorageConnection")] Stream incomingPlate,
                                      ILogger log)
        {
            var licensePlateText = string.Empty;

            try
            {
                if (incomingPlate != null)
                {
                    var createdEvent = JsonConvert.DeserializeObject<StorageBlobCreatedEventData>(eventGridEvent.Data.ToString());
                    var name = GetBlobNameFromUrl(createdEvent.Url);

                    log.LogInformation($"Processing {name}");

                    byte[] licensePlateImage;
                    // Convert the incoming image stream to a byte array.
                    using (var br = new BinaryReader(incomingPlate))
                    {
                        licensePlateImage = br.ReadBytes((int)incomingPlate.Length);
                    }

                    licensePlateText = await _cvClient.GetLicensePlate(licensePlateImage);

                    // Send the details to Event Grid.
                    await _eventGridPublisher.SendLicensePlateData(new LicensePlateData()
                    {
                        FileName = name,
                        LicensePlateText = licensePlateText,
                        TimeStamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                log.LogCritical(ex.Message);
                throw;
            }

            log.LogInformation($"Finished processing. Detected the following license plate: {licensePlateText}");
        }
    }
}
