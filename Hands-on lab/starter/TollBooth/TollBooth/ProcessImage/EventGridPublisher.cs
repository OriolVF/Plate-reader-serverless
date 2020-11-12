using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using TollBooth.Models;

namespace TollBooth
{
    public class EventGridPublisher
    {
        private readonly EventGridClient _client;
        private readonly ILogger _log;
        private readonly string _topicHostname;

        public EventGridPublisher(ILoggerFactory loggerFactory, EventGridClient client)
        {
            _topicHostname = new Uri("https://workshop-sample.westeurope-1.eventgrid.azure.net").Host;
            _log = loggerFactory.CreateLogger<EventGridPublisher>();
            _client = client;
        }

        public async Task SendLicensePlateData(LicensePlateData data)
        {
            // Will send to one of two routes, depending on success.
            // Event listeners will filter and act on events they need to
            // process (save to database, move to manual checkup queue, etc.)
            if (data.LicensePlateFound)
            {
                await Send("savePlateData", "TollBooth/CustomerService", data);
            }
            else
            {
                await Send("queuePlateForManualCheckup", "TollBooth/CustomerService", data);
            }
        }

        private async Task Send(string eventType, string subject, LicensePlateData data)
        {
            _log.LogInformation($"Sending license plate data to the {eventType} Event Grid type");

            var events = new List<EventGridEvent>()
            {
                new EventGridEvent()
                {
                    Data = data,
                    EventTime = DateTime.UtcNow,
                    EventType = eventType,
                    Id = Guid.NewGuid().ToString(),
                    Subject = subject,
                    DataVersion = "v1"
                }
            };
            await _client.PublishEventsAsync(_topicHostname, events);

            _log.LogInformation($"Sent the following to the Event Grid topic: {events[0]}");
        }
    }
}
