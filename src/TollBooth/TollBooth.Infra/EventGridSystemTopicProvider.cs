using Pulumi;
using Pulumi.Azure.EventGrid;

namespace TollBooth.Infra
{
    public class EventGridSystemTopicProvider
    {
        private SystemTopic _topic;

        public EventGridSystemTopicProvider(Input<string> resourceGroupName, string name, Input<string> resourceId)
        {
            _topic = new SystemTopic(name, new SystemTopicArgs()
            {
                ResourceGroupName = resourceGroupName,
                TopicType = "Microsoft.Storage.StorageAccounts",
                SourceArmResourceId = resourceId
            });
        }
    }
}