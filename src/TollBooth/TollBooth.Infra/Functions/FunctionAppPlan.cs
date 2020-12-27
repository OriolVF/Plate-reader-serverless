using System.Collections.Generic;
using Pulumi;
using Pulumi.Azure.AppService;

namespace TollBooth.Infra
{
    public class FunctionAppPlan : IFunctionAppPlan
    {
        public FunctionAppPlan(Plan appPlan, string tier, string kind)
        {
            Tier = tier;
            Kind = kind;
            ResourceGroupName = appPlan.ResourceGroupName;
            Id = appPlan.Id;
        }

        public string Tier { get; }
        public string Kind { get; }
        public Output<string> ResourceGroupName { get; }
        public IStorageAccount StorageAccount { get; set; }
        public Input<string> Id { get; }

        public IFunction CreateFunction(string name, Dictionary<string, string> appSettings)
        {
            StorageAccount = StorageAccountFactory.CreateStorageAccount(ResourceGroupName, new StorageAccountArgs()
            {
                Name = $"{name}-strg",
                Tier = "LRS",
                ReplicationType = "Standard"
            });

            var app = new Pulumi.Azure.AppService.FunctionApp(name, new FunctionAppArgs
            {
                ResourceGroupName = ResourceGroupName,
                AppServicePlanId = Id,
                StorageConnectionString = StorageAccount.ConnectionString,
                Version = "~2",
                AppSettings = appSettings
            });

            return new Function(app);

        }
    }

}