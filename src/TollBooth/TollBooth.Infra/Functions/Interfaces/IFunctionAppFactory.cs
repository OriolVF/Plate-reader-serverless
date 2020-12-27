using Pulumi;

namespace TollBooth.Infra
{
    public interface IFunctionAppFactory
    {
        public IFunctionAppPlan CreateAppPlan(string name, Input<string> resourceGroupName, FunctionAppPlanArgs args);
    }

}