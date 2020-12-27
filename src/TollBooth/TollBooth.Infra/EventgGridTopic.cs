using Pulumi;
using Pulumi.Azure.EventGrid;

namespace TollBooth.Infra
{
    public class EventgGridTopic
    {
        private Topic _topic;

        public EventgGridTopic(Input<string> resourceGroupName, string name)
        {
            _topic = new Topic(name, new TopicArgs()
            {
                ResourceGroupName = resourceGroupName,
            });
        }
    }
}