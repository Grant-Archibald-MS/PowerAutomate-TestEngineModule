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

            var values = new Dictionary<string, ValueContainer>();

            var formattedJson = json.Value.Replace("'", "\"");

            if ( string.IsNullOrEmpty( formattedJson ) )
            {
                throw new FormatException("Null json provided");
            }

            JsonNode value = JsonNode.Parse(formattedJson);

            AddField(values, value, "");

            var result = _state.FlowRunner?.Trigger(new ValueContainer(values)).Result;

            _logger.LogInformation("Successfully finished executing TriggerWorkflow function.");

            return RecordValue.NewRecordFromFields(
                new NamedValue("NumberOfExecutedActions", FormulaValue.New(result?.NumberOfExecutedActions))
                );
        }

        private void AddField(Dictionary<string, ValueContainer> values, JsonNode? node, string parent = "")
        {
            if ( node == null )
            {
                return;
            }

            if ( node is JsonObject )
            {
                var nodeObject = node as JsonObject;
                var items = nodeObject?.GetEnumerator();
                if ( items != null )
                {
                    while (items.MoveNext())
                    {
                        var item = items.Current;
                        if (item.Value is JsonValue)
                        {
                            var value = (JsonValue)item.Value;

                            var key = item.Key;
                            if ( ! String.IsNullOrEmpty(parent))
                            {
                                key = parent + "/" + key;
                            }

                            // TODO handle other type
                            switch ( value.GetValue<JsonElement>().ValueKind )
                            {
                                case JsonValueKind.String:
                                    values.Add(key, new ValueContainer(value.GetValue<string>()));
                                    break;
                                case JsonValueKind.True:
                                case JsonValueKind.False:
                                    values.Add(key, new ValueContainer(value.GetValue<bool>()));
                                    break;
                                case JsonValueKind.Number:
                                    values.Add(key, new ValueContainer(value.GetValue<int>()));
                                    break;
                            }
                        }
                        if (item.Value is JsonObject)
                        {
                            var key = item.Key;
                            if (!String.IsNullOrEmpty(parent))
                            {
                                key = parent + "/" + key;
                            }
                            AddField(values, item.Value, key);
                        }
                    }
                }
            }   
        }

    }
}
