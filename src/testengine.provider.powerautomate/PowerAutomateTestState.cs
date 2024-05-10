using Microsoft.Extensions.DependencyInjection;
using Parser.FlowParser;

namespace testengine.module
{
    public class PowerAutomateTestState {
        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public ServiceProvider? Provider { get; set; }
        public IFlowRunner? FlowRunner { get; set; }
    }
}