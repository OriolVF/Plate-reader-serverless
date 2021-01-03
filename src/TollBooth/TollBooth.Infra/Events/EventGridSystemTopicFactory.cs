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
}