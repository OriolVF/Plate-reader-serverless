using System.Collections.Generic;
using System.IO;
using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.Storage;

namespace TollBooth.Infra
{
    public class FunctionAppPlan : IFunctionAppPlan
    {
        public FunctionAppPlan(Plan appPlan, string tier)
        {
            Tier = tier;
            ResourceGroupName = appPlan.ResourceGroupName;
            Id = appPlan.Id;
        }

        public string Tier { get; }
        public string Kind { get; }
        public Output<string> ResourceGroupName { get; }
        public Input<string> Id { get; }

        public IFunction CreateFunction(string name, Dictionary<string, Output<string>> appSettings, DeploymentArgs args = null)
        {
            var storageAccount = StorageAccountFactory.Create(ResourceGroupName, new StorageAccountArgs()
            {
                Name = $"{name}strg",
                Tier = "Standard",
                ReplicationType = "LRS"
            });

            if (args != null)
            {
                var containerName = "myfunctions";
                storageAccount.CreateContainer(containerName);

                //Path.Combine("..", "TollBooth", "bin", "Release", "netcoreapp3.1", "publish");

                var blob = new Blob("myfunctions", new BlobArgs
                {
                    StorageAccountName = storageAccount.Name,
                    StorageContainerName = containerName,
                    Source = new FileArchive(args.PathToApp),
                    Type = "Block"
                });

                appSettings.Add("WEBSITE_RUN_FROM_PACKAGE", GetSignedBlobUrl(blob, storageAccount));
            }
            
            var app = new FunctionApp(name, new FunctionAppArgs
            {
                ResourceGroupName = ResourceGroupName,
                AppServicePlanId = Id,
                StorageConnectionString = storageAccount.ConnectionString,
                Version = "~2",
                AppSettings = Map(appSettings)
            });
            return new Function(app);
        }

        private InputMap<string> Map(Dictionary<string, Output<string>> appSettings)
        {
            var input = new InputMap<string>();
            foreach (var setting in appSettings)
            {
                input.Add(setting.Key, setting.Value);
            }

            return input;
        }

        private Input<string> GetSignedBlobUrl(Blob blob, IStorageAccount storageAccount)
        {
            const string signatureExpiration = "2100-01-01";

            var url = Output.All(new[] { storageAccount.Name, storageAccount.ConnectionString, blob.StorageContainerName, blob.Name })
            .Apply(async (parameters) =>
            {
                var accountName = parameters[0];
                var connectionString = parameters[1];
                var containerName = parameters[2];
                var blobName = parameters[3];

                var sas = await GetAccountBlobContainerSAS.InvokeAsync(new GetAccountBlobContainerSASArgs
                {
                    ConnectionString = connectionString,
                    ContainerName = containerName,
                    Start = "2020-07-20",
                    Expiry = signatureExpiration,
                    Permissions = new Pulumi.Azure.Storage.Inputs.GetAccountBlobContainerSASPermissionsArgs
                    {
                        Read = true,
                        Write = false,
                        Delete = false,
                        List = false,
                        Add = false,
                        Create = false
                    }
                });
                return $"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}{sas.Sas}";
            });

            return url;
        }
    }

    public class DeploymentArgs
    {
        public string PathToApp { get; set; }
    }


}