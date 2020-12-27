using Pulumi;
using Pulumi.Azure.Cognitive;

namespace TollBooth.Infra
{
    public class ComputerVisionProvider
    {
        private Account _account;

        public ComputerVisionProvider(string name, Input<string> resourceGroupName)
        {
            _account = new Account(name, new AccountArgs()
            {
                ResourceGroupName = resourceGroupName,
                Kind = "ComputerVision",
                SkuName = "S1"
            });
        }
    }
}