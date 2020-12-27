using Pulumi;

namespace TollBooth.Infra
{
    public interface IFunction
    {
        public Output<string> DefaultHostName { get; }
        public Output<string> Id { get; }
    }

}