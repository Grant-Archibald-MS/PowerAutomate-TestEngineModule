using Parser.ExpressionParser;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using YamlDotNet.Core.Tokens;


namespace testengine.module.powerautomate
{
    public class ValueContainerConvertor
    {
        RecalcEngine _engine = null;

        public ValueContainerConvertor()
        {
            var powerFxConfig = new PowerFxConfig(Features.PowerFxV1);
            _engine = new RecalcEngine(powerFxConfig);
        }

        public Dictionary<string, ValueContainer> Convert(string json)
        {
            Dictionary<string, ValueContainer> results = new Dictionary<string, ValueContainer>();

            // Handle embedded quotes '' and convert to escaped quote \"
            var formattedJson = json.Replace("''", "\\\"");
            // Convert single quote ' into "
            formattedJson = formattedJson.Replace("'", "\"");

            if (string.IsNullOrEmpty(formattedJson))
            {
                throw new FormatException("Null json provided");
            }

            JsonNode value = JsonNode.Parse(formattedJson);

            AddPowerFxState(value);

            AddField(results, value, "");

            return results;
        }

        private void AddPowerFxState(JsonNode? node, string parent = "")
        {
            if (node is JsonObject)
            {
                var nodeObject = node as JsonObject;
                var items = nodeObject?.GetEnumerator();
                if (items != null)
                {
                    while (items.MoveNext())
                    {
                        var item = items.Current;
                        if (item.Value is JsonValue)
                        {
                            var value = (JsonValue)item.Value;

                            var key = item.Key;
                            if (!String.IsNullOrEmpty(parent))
                            {
                                key = parent + "_" + key;
                            }

                            switch (value.GetValue<JsonElement>().ValueKind)
                            {
                                case JsonValueKind.String:
                                    _engine.UpdateVariable(key, value.ToString());
                                    break;
                                case JsonValueKind.True:
                                case JsonValueKind.False:
                                    _engine.UpdateVariable(key, value.GetValue<bool>());
                                    break;
                                case JsonValueKind.Number:
                                    try
                                    {
                                        _engine.UpdateVariable(key, value.GetValue<int>());
                                    }
                                    catch
                                    {
                                        _engine.UpdateVariable(key, value.GetValue<double>());
                                    }
                                    break;
                            }
                        }

                        if (item.Value is JsonObject)
                        {
                            var key = item.Key;
                            if (!String.IsNullOrEmpty(parent))
                            {
                                key = parent + "_" + key;
                            }
                            AddPowerFxState(item.Value, key);
                        }
                    }
                }
            }
        }

        private void AddField(Dictionary<string, ValueContainer> values, JsonNode? node, string parent = "")
        {
            if (node == null)
            {
                return;
            }

            if (node is JsonObject)
            {
                var nodeObject = node as JsonObject;
                var items = nodeObject?.GetEnumerator();
                if (items != null)
                {
                    while (items.MoveNext())
                    {
                        var item = items.Current;
                        if (item.Value is JsonValue)
                        {
                            var value = (JsonValue)item.Value;

                            var key = item.Key;
                            if (!String.IsNullOrEmpty(parent))
                            {
                                key = parent + "/" + key;
                            }

                            switch (value.GetValue<JsonElement>().ValueKind)
                            {
                                case JsonValueKind.String:
                                    var text = value.GetValue<string>();
                                    if ( text.StartsWith("=") )
                                    {
                                        var newValue = _engine.Eval(text.Substring(1));
                                        if (newValue is DateValue d)
                                        {
                                            values.Add(key, new ValueContainer(d.GetConvertedValue(TimeZoneInfo.Utc)));
                                        }
                                        if ( newValue is StringValue )
                                        {
                                            values.Add(key, new ValueContainer(newValue.ToString()));
                                        }
                                        if (newValue is BooleanValue)
                                        {
                                            values.Add(key, new ValueContainer(newValue.AsBoolean()));
                                        }
                                        if (newValue is NumberValue)
                                        {
                                            values.Add(key, new ValueContainer(newValue.AsDouble()));
                                        }
                                        if (newValue is DecimalValue)
                                        {
                                            try
                                            {
                                                values.Add(key, new ValueContainer((int)newValue.AsDecimal()));
                                            }
                                            catch
                                            {
                                                values.Add(key, new ValueContainer(newValue.AsDouble()));
                                            }
                                        }
                                        if (newValue is DateTimeValue)
                                        {
                                            values.Add(key, new ValueContainer(newValue.ToString()));
                                        }
                                        if (newValue is GuidValue g)
                                        {
                                            values.Add(key, new ValueContainer(g.Value));
                                        }
                                    } 
                                    else
                                    {
                                        values.Add(key, new ValueContainer(text));
                                    }
                                    break;
                                case JsonValueKind.True:
                                case JsonValueKind.False:
                                    values.Add(key, new ValueContainer(value.GetValue<bool>()));
                                    break;
                                case JsonValueKind.Number:
                                    try
                                    {
                                        values.Add(key, new ValueContainer(value.GetValue<int>()));
                                    } catch
                                    {
                                        values.Add(key, new ValueContainer(value.GetValue<double>()));
                                    }
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
