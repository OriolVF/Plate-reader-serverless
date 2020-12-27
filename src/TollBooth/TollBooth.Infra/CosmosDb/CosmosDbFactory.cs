using Pulumi;
using Pulumi.Azure.CosmosDB;
using Pulumi.Azure.CosmosDB.Inputs;

namespace TollBooth.Infra
{
    public static class CosmosDbFactory
    {
        public static ICosmosDb CreateSqlDb(string name, Output<string> resourceGroupName)
        {
            var account = new Account($"{name}-account", new AccountArgs()
            {
                ResourceGroupName = resourceGroupName,
                OfferType = "Standard",
                Kind = "GlobalDocumentDB",
                EnableAutomaticFailover = true,
                Capabilities = new InputList<AccountCapabilityArgs>()
                {
                    new AccountCapabilityArgs()
                    {
                        Name = "EnableServerless"
                    }
                },
                ConsistencyPolicy = new AccountConsistencyPolicyArgs()
                {
                    ConsistencyLevel = "BoundedStaleness",
                    MaxIntervalInSeconds = 10,
                    MaxStalenessPrefix = 200
                },
                GeoLocations = new InputList<AccountGeoLocationArgs>()
                {
                    new AccountGeoLocationArgs()
                    {
                        FailoverPriority = 0,
                        Location = "westeurope"
                    }
                },
            });

            var db = new SqlDatabase(name, new SqlDatabaseArgs()
            {
                AccountName = account.Name,
                ResourceGroupName = resourceGroupName
            });
            return new CosmosDbSqlDb(db, account);
        }
    }
}