using Pulumi;

namespace TollBooth.Infra
{
    public interface ICosmosDb
    {
        void CreateContainer(string name, string partitionKeyPath);
        public Output<string> AccountName { get; }
        public Output<string> DbName { get; }
        public Output<string> AccessKey { get; }
        public Output<string> PrimaryEndpoint { get; }
    }
}