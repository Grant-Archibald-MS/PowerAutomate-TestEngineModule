// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerFx;
using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Config;
using Microsoft.PowerApps.TestEngine.Modules;
using Microsoft.PowerApps.TestEngine.Providers;
using Microsoft.PowerApps.TestEngine.System;
using Microsoft.PowerApps.TestEngine.TestInfra;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.DependencyInjection;

namespace testengine.module
{
    [Export(typeof(ITestEngineModule))]
    public class PowerAutomateTestModule : ITestEngineModule
    {
        public void ExtendBrowserContextOptions(BrowserNewContextOptions options, TestSettings settings)
        {

        }

        public void RegisterPowerFxFunction(PowerFxConfig config, ITestInfraFunctions testInfraFunctions, ITestWebProvider testWebProvider, ISingleTestInstanceState singleTestInstanceState, ITestState testState, IFileSystem fileSystem)
        {
            ILogger logger = singleTestInstanceState.GetLogger();

            var state = testWebProvider.ProviderState?.GetState() as PowerAutomateTestState;

            if ( state == null )
            {
                logger.LogError("Unable to get provide test state");
                return;
            }

            config.AddFunction(new BuildTestFunction(state, logger));
            config.AddFunction(new OpenWorkflowFunction(state, logger));
            config.AddFunction(new MockActionFunction(state, logger));
            config.AddFunction(new TriggerWorkflowFunction(state, logger));
            
            logger.LogInformation("Registered BuildTest()");
        }

        public async Task RegisterNetworkRoute(ITestState state, ISingleTestInstanceState singleTestInstanceState, IFileSystem fileSystem, IPage Page, NetworkRequestMock mock)
        {
            await Task.CompletedTask;
        }
    }
}