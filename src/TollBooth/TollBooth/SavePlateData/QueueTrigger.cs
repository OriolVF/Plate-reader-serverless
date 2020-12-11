using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TollBooth.SavePlateData
{
    public class QueueTrigger
    {
        [FunctionName("Queuetrigger")]
        public async Task Run([ServiceBusTrigger("servicebus_queue", Connection = "ServiceBusConnection")]
            string myQueueItem,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log)
        {
            log.LogInformation($"Message received {messageId}");
        }
    }
}