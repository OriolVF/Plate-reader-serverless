using Pulumi;
using Pulumi.Azure.Storage;

namespace TollBooth.Infra
{

    public class StorageAccountFactory
    {
        public static IStorageAccount Create(Input<string> resourceGroupName, StorageAccountArgs args)
        {
            var storageAccount = new Pulumi.Azure.Storage.Account(args.Name, new AccountArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountReplicationType = args.ReplicationType,
                AccountTier = args.Tier,
            });

            return new StorageAccount(storageAccount.Name, storageAccount.PrimaryConnectionString, storageAccount.Id);
        }
    }
}
