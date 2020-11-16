using System;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(TollBooth.Startup))]

namespace TollBooth
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddTransient(_ =>
            {
                var topicKey = Environment.GetEnvironmentVariable("eventGridTopicKey");
                var topicCredentials = new TopicCredentials(topicKey);
                return new EventGridClient(topicCredentials);
            });
            builder.Services.AddTransient<ComputerVisionClient>();
            builder.Services.AddTransient<EventGridPublisher>();
        }
    }
}