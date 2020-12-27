using Pulumi;
using Pulumi.Azure.Storage;

namespace TollBooth.Infra
{
    public class StorageProvider
    {
        public readonly Account StorageAccount;

        public StorageProvider(Input<string> resourceGroupName, string accountName)
        {
            StorageAccount = new Account(accountName, new AccountArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountReplicationType = "LRS",
                AccountTier = "Standard",
            });
        }

        public void AddContainer(string name)
        {
            var container = new Container(name, new ContainerArgs
            {
                StorageAccountName = StorageAccount.Name,
            });
        }
    }


    public static class StorageAccountFactory
    {
        public static IStorageAccount CreateStorageAccount(Input<string> resourceGroupName, StorageAccountArgs args)
        {
            var account = new Account(args.Name, new AccountArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountReplicationType = args.ReplicationType,
                AccountTier = args.Tier,
            });

            return new StorageAccount(account);
        }
    }

    public interface IStorageAccount
    {
        public Output<string> Id { get; }
        public Output<string> ConnectionString { get; }
        public Output<string> Name { get; }
        void CreateContainer(string name);
    }

    public class StorageAccount : IStorageAccount
    {
        public Output<string> Id { get; }
        public Output<string> ConnectionString { get; }
        public Output<string> Name { get; }

        public StorageAccount(Account account)
        {
            Id = account.Id;
            ConnectionString = account.PrimaryConnectionString;
            Name = account.Name;
        }

        public void CreateContainer(string name)
        {
            var container = new Container(name, new ContainerArgs
            {
                StorageAccountName = Name,
            });
        }
    }

    public class StorageAccountArgs
    {
        public string ReplicationType { get; set; }
        public string Tier { get; set; }
        public string Name { get; set; }
    }
}
