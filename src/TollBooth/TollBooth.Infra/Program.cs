using System;
using System.Threading.Tasks;
using Pulumi;
namespace TollBooth.Infra
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}
