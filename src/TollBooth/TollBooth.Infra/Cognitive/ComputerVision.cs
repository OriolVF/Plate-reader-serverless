using Pulumi;

namespace TollBooth.Infra
{

    public class ComputerVision
    {
        public Output<string> EndpointUrl { get; }
        public Output<string> ApiKey { get; set; }

        public ComputerVision(Output<string> endpointUrl, Output<string> apiKey)
        {
            EndpointUrl = endpointUrl;
            ApiKey = apiKey;
        }

    }
}