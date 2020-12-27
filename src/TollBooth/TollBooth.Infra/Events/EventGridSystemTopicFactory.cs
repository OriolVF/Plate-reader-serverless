using Pulumi;
using Pulumi.Azure.EventGrid;

namespace TollBooth.Infra
{
    public static class EventGridSystemTopicFactory
    {
        public static Output<string> Create(Output<string> resourceGroupName, string name, Output<string> resourceId)
        {
            var topic = new SystemTopic(name, new SystemTopicArgs()
            {
                ResourceGroupName = resourceGroupName,
                TopicType = "Microsoft.Storage.StorageAccounts",
                SourceArmResourceId = resourceId
            });

            return topic.Id;
        }
    }
}