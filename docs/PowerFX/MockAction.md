# MockAction

`MockAction(ActionName,BooleanExpression)`

The MockAction function creates an expected mock outcome when unit testing a Power Automate Cloud flow.

If the boolean expression is `true` then the action will set as successfully completed. If the expression is `false` then the action will set as unsuccessful.

## Example

`MockAction("Some_Action", true);`

`MockAction("Some_Action", false);`
