// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerFx.Types;
using Microsoft.PowerFx;
using Microsoft.Extensions.Logging;
using Parser.FlowParser;
using Microsoft.Extensions.DependencyInjection;
using Parser;

namespace testengine.module
{
    /// <summary>
    /// This will combined the test setup so that the workflow can be tested
    /// </summary>
    public class BuildTestFunction : ReflectionFunction
    {
        private readonly PowerAutomateTestState _state;
        private readonly ILogger _logger;

        public BuildTestFunction(PowerAutomateTestState state, ILogger logger)
            : base("BuildTest", FormulaType.Blank)
        {
            _state = state;
            _logger = logger;
        }

        public BlankValue Execute()
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing BuildTest function.");

            _state.Services.AddFlowRunner();
            _state.Provider = _state.Services.BuildServiceProvider();
            _state.FlowRunner = _state.Provider.GetRequiredService<IFlowRunner>();
            
            _logger.LogInformation("Successfully finished executing Pause function.");

            return FormulaValue.NewBlank();
        }
    }
}

