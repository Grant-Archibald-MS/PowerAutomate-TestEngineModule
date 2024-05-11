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
            : base("TriggerWorkflow", FormulaType.Blank, StringType.String)
        {
            _state = state;
            _logger = logger;
        }

        public BlankValue Execute(StringValue source)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing OpenWorkflow function.");

            var values = new Dictionary<string, ValueContainer>();

            var data = source.ToObject();

            // TODO
            //var fields = source.GetFieldsAsync(new CancellationToken()).ToEnumerable();

            //foreach ( var field in fields )
            //{
            //    AddField(values, String.Empty, field.Name, field.Value);
            //}

            //
            var result = _state.FlowRunner?.Trigger(new ValueContainer(values));

            _logger.LogInformation("Successfully finished executing TriggerWorkflow function.");

            return FormulaValue.NewBlank();
        }

        private void AddField(Dictionary<string, ValueContainer> values, string parent, string name, FormulaValue value)
        {
            var fieldName = String.Empty;
            if ( !String.IsNullOrEmpty(parent) && !String.IsNullOrEmpty(name) )
            {
                fieldName = $"{parent}/{name}";
            } else
            {
                if (!String.IsNullOrEmpty(parent) && String.IsNullOrEmpty(name))
                {
                    fieldName = parent;
                } else
                {
                    fieldName = name;
                }
            }

            if (value is RecordValue)
            {
                AddField(values, fieldName, string.Empty, value as RecordValue);
            }
            if ( value is StringValue)
            {
                values.Add(fieldName, new ValueContainer((value as StringValue)?.Value));
            }
            if (value is BooleanValue)
            {
                values.Add(fieldName, new ValueContainer((value as BooleanValue)?.Value));
            }
            if (value is NumberValue)
            {
                values.Add(fieldName, new ValueContainer((value as BooleanValue)?.Value));
            }
            if (value is DateTimeValue)
            {
                values.Add(fieldName, new ValueContainer((value as DateTimeValue)?.Value));
            }
            if (value is DateValue)
            {
                values.Add(fieldName, new ValueContainer((value as DateValue)?.Value));
            }
        }

        private void AddField(Dictionary<string, ValueContainer> values, string parent, string name, RecordValue? recordValue)
        {
            var fields = recordValue?.GetFieldsAsync(new CancellationToken()).ToEnumerable();
            foreach (var recordField in fields)
            {
                AddField(values, parent, recordField.Name, recordField.Value);
            }
        }
    }
}
