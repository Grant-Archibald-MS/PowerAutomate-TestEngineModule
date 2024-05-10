using Microsoft.PowerApps.TestEngine.Providers;
using testengine.module;

namespace testengine.provider.powerautomate
{
    public class PowerAutomateTestStateProvider : ITestProviderState
    {
        PowerAutomateTestState _state = new PowerAutomateTestState();

        public object GetState()
        {
            return _state;
        }
    }
}
