using Pulumi;

namespace TollBooth.Infra
{
    public interface IStorageAccount
    {
        void CreateContainer(string containerName);
        Output<string> ConnectionString { get; }
        Output<string> Id { get; }
    }
}
