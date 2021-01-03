using Pulumi;

namespace TollBooth.Infra
{
    public class StorageAccountArgs
    {
        public Input<string> ReplicationType { get; internal set; }
        public Input<string> Tier { get; internal set; }
        public string Name { get; internal set; }
    }
}