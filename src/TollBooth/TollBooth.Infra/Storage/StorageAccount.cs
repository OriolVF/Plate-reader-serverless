using Pulumi;
using Pulumi.Azure.Storage;

namespace TollBooth.Infra
{
    public class StorageAccount : IStorageAccount
    {
        private Input<string> Name { get; }

        public Output<string> ConnectionString { get; }

        public Output<string> Id { get; }

        public StorageAccount(Input<string> name, Output<string> connectionString, Output<string> id)
        {
            Name = name;
            ConnectionString = connectionString;
            Id = id;
        }

        public void CreateContainer(string containerName)
        {
            var container = new Container(containerName, new Pulumi.Azure.Storage.ContainerArgs
            {
                Name = containerName,
                StorageAccountName = Name,
            });
        }
    }
}
