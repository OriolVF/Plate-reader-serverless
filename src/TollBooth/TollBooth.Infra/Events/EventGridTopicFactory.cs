using Pulumi;
using Pulumi.Azure.EventGrid;

namespace TollBooth.Infra
{
    public static class EventGridTopicFactory
    {
        public static EventGridTopic Create(Output<string> resourceGroupName, string name)
        {
            var topic = new Topic(name, new TopicArgs()
            {
                ResourceGroupName = resourceGroupName,
            });

            return new EventGridTopic(topic.Endpoint, topic.PrimaryAccessKey, topic.Name);
        }
    }
}