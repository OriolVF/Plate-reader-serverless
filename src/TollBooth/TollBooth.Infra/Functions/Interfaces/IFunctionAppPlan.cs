using Pulumi;
using System.Collections.Generic;

namespace TollBooth.Infra
{
    public interface IFunctionAppPlan
    {
        public IFunction CreateFunction(string name, Dictionary<string, Output<string>> appSettings);
    }

}