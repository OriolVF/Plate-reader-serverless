using Pulumi;
using Pulumi.Azure.CosmosDB;
using Pulumi.Azure.CosmosDB.Inputs;

namespace TollBooth.Infra
{
    public class CosmosDbSqlDb : ICosmosDb
    {
        public CosmosDbSqlDb(SqlDatabase db, Account account)
        {
            AccountName = db.AccountName;
            DbName = db.Name;
            AccessKey = account.PrimaryKey;
            PrimaryEndpoint = account.Endpoint;
            ResourceGroupName = db.ResourceGroupName;
        }

        public Output<string> AccountName { get; }
        public Output<string> DbName { get; }
        public Output<string> AccessKey { get; }
        public Output<string> PrimaryEndpoint { get; }
        public Output<string> ResourceGroupName { get; }

        public void CreateContainer(string name, string partitionKeyPath)
        {
            var container  = new SqlContainer(name, new SqlContainerArgs()
            {
                AccountName = AccountName,
                ResourceGroupName = ResourceGroupName,
                DatabaseName = DbName,
                PartitionKeyPath = partitionKeyPath,
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