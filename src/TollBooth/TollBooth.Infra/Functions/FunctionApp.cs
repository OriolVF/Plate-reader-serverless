using System.Collections.Generic;
using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.Storage;

namespace TollBooth.Infra
{
    public class FunctionApp
    {
        private readonly Account _storageAccount;
        private readonly Plan _plan;
        private readonly Input<string> _resourceGroupName;

        public FunctionApp(string appPlanName, Input<string> resourceGroupName)
        {
            _resourceGroupName = resourceGroupName;
            _storageAccount = new Account($"{appPlanName}", new AccountArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountReplicationType = "LRS",
                AccountTier = "Standard",
            });

            _plan = new Plan($"asp-{appPlanName}", new PlanArgs
            {
                ResourceGroupName = resourceGroupName,
                Kind = "FunctionApp",
                Sku = new Pulumi.Azure.AppService.Inputs.PlanSkuArgs
                {
                    Tier = "Dynamic",
                    Size = "Y1"
                }
            });
        }
        public Output<string> AddFunction(string name, Dictionary<string, string> appSettings)
        {
            var app = new Pulumi.Azure.AppService.FunctionApp(name, new FunctionAppArgs
            {
                ResourceGroupName = _resourceGroupName,
                AppServicePlanId = _plan.Id,
                StorageConnectionString = _storageAccount.PrimaryConnectionString,
                Version = "~2",
                AppSettings = appSettings
            });

            return app.DefaultHostname;
        }
    }

}