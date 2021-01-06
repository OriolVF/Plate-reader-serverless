using Pulumi;

namespace TollBooth.Infra
{
    public class EventGridTopic
    {
        public Output<string> Endpoint { get; }
        public Output<string> Key { get; }
        public Output<string> Name { get; set; }


        public EventGridTopic(Input<string> endpoint, Input<string> key, Input<string> name)
        {
            Endpoint = endpoint;
            Key = key;
            Name = name;
        }

    }
}