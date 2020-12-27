using Pulumi;
using Pulumi.Azure.AppService;

namespace TollBooth.Infra
{
    public static class FunctionAppFactory
    {
        public static IFunctionAppPlan CreateAppPlan(string name, Input<string> resourceGroupName, FunctionAppPlanArgs args)
        {
            var plan = new Plan($"asp-{name}", new PlanArgs
            {
                ResourceGroupName = resourceGroupName,
                Kind = "FunctionApp",
                Sku = new Pulumi.Azure.AppService.Inputs.PlanSkuArgs
                {
                    Tier = args.Tier,
                    Size = args.Size
                }
            });

            return new FunctionAppPlan(plan, args.Tier);
        }
    }

}