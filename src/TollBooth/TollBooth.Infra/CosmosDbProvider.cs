using Pulumi;
using Pulumi.Azure.CosmosDB;
using Pulumi.Azure.CosmosDB.Inputs;

namespace TollBooth.Infra
{
    public class CosmosDbProvider
    {
        private readonly Account _account;
        private SqlDatabase _db;
        private SqlContainer _container;

        public CosmosDbProvider(Input<string> resourceGroupName, string accountName )
        {
            _account = new Account(accountName, new AccountArgs()
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
        }
        
        public void CreateSqlDb(string dbName)
        {
            _db = new SqlDatabase(dbName, new SqlDatabaseArgs()
            {
                AccountName = _account.Name,

            });
        }

        public void CreateContainer(string name, string partionKeyPath, string indexingMode)
        {
            _container = new SqlContainer(name, new SqlContainerArgs()
            {
                AccountName = _account.Name,
                DatabaseName = _db.Name,
                PartitionKeyPath = partionKeyPath,
                IndexingPolicy = new SqlContainerIndexingPolicyArgs()
                {
                    ExcludedPaths = new[]
                    {
                        new SqlContainerIndexingPolicyExcludedPathArgs()
                        {
                            Path = "/excluded/?"
                        }
                    },
                    IncludedPaths = new[]
                    {
                        new SqlContainerIndexingPolicyIncludedPathArgs()
                        {
                            Path = "/included/?"
                        },
                        new SqlContainerIndexingPolicyIncludedPathArgs()
                        {
                            Path = "/*"
                        }
                    },
                    IndexingMode = "Consistent"
                },
                UniqueKeys = new[]
                {
                    new SqlContainerUniqueKeyArgs()
                    {
                        Paths = new[]
                        {
                            "/definition/idlong",
                            "/definition/idshort"
                        }
                    }
                }
            });
        }
    }
}