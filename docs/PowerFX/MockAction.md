# MockAction

`MockAction(ActionName,BooleanExpression, json)`

The MockAction function creates an expected mock outcome when unit testing a Power Automate Cloud flow.

If the boolean expression is `true` then the action will set as successfully completed. If the expression is `false` then the action will set as unsuccessful.

The option json argument is used to set the Trigger output

## Example

`MockAction("Some_Action", true);`

`MockAction("Some_Action", false);`

`MockAction("Some_Action", true, "{'name':'Test'});`