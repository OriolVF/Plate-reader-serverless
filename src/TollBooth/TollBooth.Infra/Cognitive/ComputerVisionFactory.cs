using Pulumi;
using Pulumi.Azure.Cognitive;

namespace TollBooth.Infra
{
    public static class ComputerVisionFactory
    {
        public static ComputerVision Create(string name, Output<string> resourceGroupName)
        {
            var account = new Account(name, new AccountArgs()
            {
                ResourceGroupName = resourceGroupName,
                Kind = "ComputerVision",
                SkuName = "S1"
            });

            return new ComputerVision(account.Endpoint, account.PrimaryAccessKey);
        }
    }
}