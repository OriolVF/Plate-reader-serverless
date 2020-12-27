using Pulumi;

namespace TollBooth.Infra
{
    public class Function : IFunction
    {
        public Function(Pulumi.Azure.AppService.FunctionApp function)
        {
            DefaultHostName = function.DefaultHostname;
            Id = function.Id;
        }

        public Output<string> DefaultHostName { get; }
        public Output<string> Id { get; }
    }

}