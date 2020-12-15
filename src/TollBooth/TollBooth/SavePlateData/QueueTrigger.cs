using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using TollBooth.Models;

namespace TollBooth.SavePlateData
{
    public class QueueTrigger
    {
        private readonly ComputerVisionClient _cvClient;
        private readonly EventGridPublisher _eventGridPublisher;

        public QueueTrigger(ComputerVisionClient cvClient, 
            EventGridPublisher eventGridPublisher)
        {
            _cvClient = cvClient;
            _eventGridPublisher = eventGridPublisher;
        }

        [FunctionName("Queuetrigger")]
        public async Task Run([ServiceBusTrigger("servicebus_queue", Connection = "ServiceBusConnection")]
            string myQueueItem,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            log.LogInformation($"Message received {myQueueItem}");
            var licensePlateText = string.Empty;
            var eventItem = JsonConvert.DeserializeObject<Rootobject>(myQueueItem);
            var incomingPlate = await GetPlateFromBlob(eventItem.data.url);
            log.LogInformation($"Image stream length {incomingPlate?.Length}");
            try
            {
                if (incomingPlate != null)
                {
                    var name = GetBlobNameFromUrl(eventItem.data.url);

                    log.LogInformation($"Processing {name}");

                    byte[] licensePlateImage;
                    // Convert the incoming image stream to a byte array.
                    using (var br = new BinaryReader(incomingPlate))
                    {
                        licensePlateImage = br.ReadBytes((int)incomingPlate.Length);
                    }

                    licensePlateText = await _cvClient.GetLicensePlate(licensePlateImage);
                    log.LogInformation($"License plate found: {licensePlateText}");
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

        private async Task<Stream> GetPlateFromBlob(string bloblUrl)
        {
            var uri = new Uri(bloblUrl);
            var cloudBlob = new CloudBlob(uri);
            var memoryStream = new MemoryStream();
            await cloudBlob.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string GetBlobNameFromUrl(string bloblUrl)
        {
            var uri = new Uri(bloblUrl);
            var cloudBlob = new CloudBlob(uri);
            return cloudBlob.Name;
        }

    }


    public class Rootobject
    {
        public string topic { get; set; }
        public string subject { get; set; }
        public string eventType { get; set; }
        public string id { get; set; }
        public Data data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
        public DateTime eventTime { get; set; }
    }

    public class Data
    {
        public string api { get; set; }
        public string clientRequestId { get; set; }
        public string requestId { get; set; }
        public string eTag { get; set; }
        public string contentType { get; set; }
        public int contentLength { get; set; }
        public string blobType { get; set; }
        public string url { get; set; }
        public string sequencer { get; set; }
        public Storagediagnostics storageDiagnostics { get; set; }
    }

    public class Storagediagnostics
    {
        public string batchId { get; set; }
    }

}