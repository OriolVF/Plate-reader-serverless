using System.Collections.Generic;
using Pulumi;
using Pulumi.Azure.AppService;

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

        public IFunction CreateFunction(string name, Dictionary<string, Output<string>> appSettings)
        {
            var storageAccount = StorageAccountFactory.Create(ResourceGroupName, new StorageAccountArgs()
            {
                Name = $"{name}strg",
                Tier = "Standard",
                ReplicationType = "LRS"
            });

            var app = new Pulumi.Azure.AppService.FunctionApp(name, new FunctionAppArgs
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
    }
}