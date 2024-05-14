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

        private static NamedFormulaType TotalNumberOfExecutedActions = new NamedFormulaType("TotalNumberOfExecutedActions", FormulaType.Number);
        private static NamedFormulaType Name = new NamedFormulaType("Name", FormulaType.String);

        private static RecordType WorkflowResult = RecordType.Empty()
            .Add(TotalNumberOfExecutedActions)
            .Add(Name);

        public TriggerWorkflowFunction(PowerAutomateTestState state, ILogger logger)
            // Function name, start with return type then arguments
            : base("TriggerWorkflow", TableType.Empty()
                  .Add(TotalNumberOfExecutedActions)
                  .Add(Name), StringType.String)
        {
            _state = state;
            _logger = logger;
        }

        public TableValue Execute(StringValue json)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing OpenWorkflow function.");

            var values = new ValueContainerConvertor().Convert(json.Value);
           
            var result = _state.FlowRunner?.Trigger(new ValueContainer(values)).Result;

            _logger.LogInformation("Successfully finished executing TriggerWorkflow function.");

            var results = new List<RecordValue>();

            if (result?.ActionStates != null)
            {
                foreach (var state in result.ActionStates)
                {
                    results.Add(RecordValue.NewRecordFromFields(
                       new NamedValue("TotalNumberOfExecutedActions", FormulaValue.New(result?.NumberOfExecutedActions)),
                       new NamedValue("Name", FormulaValue.New(state.Key))
                   )); ;
                }
            }
           
            return TableValue.NewTable(WorkflowResult, results);
        }
    }
}
