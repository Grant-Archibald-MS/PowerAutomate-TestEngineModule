using Microsoft.Extensions.Logging;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using Parser.ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Text.Json.Nodes;
using System.Text.Json;
using testengine.module.powerautomate;

namespace testengine.module
{
    // Must end in Function
    public class TriggerWorkflowFunction : ReflectionFunction
    {
        private readonly PowerAutomateTestState _state;
        private readonly ILogger _logger;

        public Func<string,string> GetFileContent = name => File.ReadAllText(name);

        public TriggerWorkflowFunction(PowerAutomateTestState state, ILogger logger)
            // Function name, start with return type then arguments
            : base("TriggerWorkflow", RecordType.Empty().Add(new NamedFormulaType("NumberOfExecutedActions", FormulaType.Number)), StringType.String)
        {
            _state = state;
            _logger = logger;
        }

        public RecordValue Execute(StringValue json)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing OpenWorkflow function.");

            var values = new ValueContainerConvertor().Convert(json.Value);
           
            var result = _state.FlowRunner?.Trigger(new ValueContainer(values)).Result;

            _logger.LogInformation("Successfully finished executing TriggerWorkflow function.");

            return RecordValue.NewRecordFromFields(
                new NamedValue("NumberOfExecutedActions", FormulaValue.New(result?.NumberOfExecutedActions))
                );
        }
    }
}
