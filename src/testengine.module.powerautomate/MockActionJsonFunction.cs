// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerFx.Types;
using Microsoft.PowerFx;
using Microsoft.Extensions.Logging;
using Parser.FlowParser;
using Microsoft.Extensions.DependencyInjection;
using Parser;
using Parser.FlowParser.ActionExecutors;
using Sprache;
using testengine.provider.powerautomate;
using testengine.module.powerautomate;
using Parser.ExpressionParser;

namespace testengine.module
{
    /// <summary>
    /// Will mock an action of a workflow
    /// </summary>
    public class MockActionJsonFunction : ReflectionFunction
    {
        private readonly PowerAutomateTestState _state;
        private readonly ILogger _logger;

        public MockActionJsonFunction(PowerAutomateTestState state, ILogger logger)
            // Function name, start with return type then arguments
            : base("MockAction", FormulaType.Blank, FormulaType.String, FormulaType.Boolean, FormulaType.String)
        {
            _state = state;
            _logger = logger;
        }

        public BlankValue Execute(StringValue name, BooleanValue continueExecution, StringValue json)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing MockAction function.");

            _state.Services.AddSingleton<ActionExecutorRegistration>(
                sp =>
                {
                    var result = new ActionResult()
                    {
                        ContinueExecution = continueExecution.Value,
                        ActionOutput = new ValueContainer(new ValueContainerConvertor().Convert(json.Value))
                    };
                    return new ActionExecutorRegistration
                    {
                        ActionName = name.Value,
                        Instance = new PowerFxActionExecutor()
                        {
                            Result = result
                        }
                    };
                }
            );
            
            _logger.LogInformation("Successfully finished executing MockAction function.");

            return FormulaValue.NewBlank();
        }
    }
}

