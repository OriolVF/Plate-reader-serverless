using Pulumi;

namespace TollBooth.Infra
{
    public class EventGridTopic
    {
        public Output<string> Endpoint { get; }
        public Output<string> Key { get; }

        public EventGridTopic(Input<string> endpoint, Input<string> key)
        {
            Endpoint = endpoint;
            Key = key;
        }
    }
}