# TriggerWorkflow

`TriggerWorkflow(json)`

The TriggerWorkflow function starts the unit test of a Power Automate Cloud Flow.

> [!NOTE]: For the JSON text single quotes for properties and text string will be converted to double quotes.

The function returns a Record with the total number actions executed. Use can use the [With](https://learn.microsoft.com/power-platform/power-fx/reference/function-with) function to assert the results.

## Example

`TriggerWorkflow("{'somedata':'text'}");`

```PowerFX
With( {
        results: TriggerWorkflow("{'body':{'name': 'Alice Bob', 'accountId': 1}}") 
    },
    Assert(2, results.NumberOfExecutedActions)
    )
```
