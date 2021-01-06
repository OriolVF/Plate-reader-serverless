using System.Dynamic;
using Pulumi;
using Pulumi.Azure.EventGrid;

namespace TollBooth.Infra
{
    public static class EventGridSystemTopicFactory
    {
        public static EventGridSystemTopic Create(Output<string> resourceGroupName, string name, Output<string> resourceId)
        {
            var topic = new SystemTopic(name, new SystemTopicArgs()
            {
                ResourceGroupName = resourceGroupName,
                TopicType = "Microsoft.Storage.StorageAccounts",
                SourceArmResourceId = resourceId
            });
            return new EventGridSystemTopic
            {
                Id = topic.Id,
                Name = topic.Name
            };
        }
    }

    public class EventGridSystemTopic
    {
        public Output<string> Id { get; set; }
        public Output<string> Name { get; set; }
    }

    public static class EventGridSystemSubscriptionFactory
    {
        public static void CreateForFunctionApp(Output<string> functionAppId, Output<string> resourceGroupName,
            Output<string> systemTopicName, string eventName, string endpointName)
        {
            var args = new Pulumi.Azure.EventGrid.Inputs.SystemTopicEventSubscriptionAzureFunctionEndpointArgs()
            {
                FunctionId = Output.Format($"{functionAppId}/functions/{endpointName}")
            };

            var systemTopicSub = new SystemTopicEventSubscription(eventName, new SystemTopicEventSubscriptionArgs()
            {
                ResourceGroupName = resourceGroupName,
                SystemTopic = systemTopicName,
                AzureFunctionEndpoint = args
            });
        }
    }

    public static class EventGridSubscriptionFactory
    {
        public static void CreateForFunctionApp(Output<string> functionAppId, Output<string> topicName,
            string eventName, string endpointName)
        {
            var processedPlatesub = new EventSubscription(eventName, new EventSubscriptionArgs()
            {
                TopicName = topicName,
                AzureFunctionEndpoint = new Pulumi.Azure.EventGrid.Inputs.EventSubscriptionAzureFunctionEndpointArgs()
                {
                    FunctionId = Output.Format($"{functionAppId}/functions/{endpointName}"),
                },
                
            });

        }
    }
}