# Overview

The basic test is designed to validate the ability to execute a multi action cloud flow that includes the following:

- Triggers. Use the [MockAction](../../docs/PowerFX/MockAction.md) to successfully to start a flow
- Mock Actions. Set the outcome and results of actions inside the cloud flow.
- Validate executed actions. Evaluate the number of actions that executed

## Run this sample

To run this sample

1. Ensure setup prerequisites and build are completed as described in [Project Readme](../../README.md)

2. Execute test

```bash
cd samples/basic
dotnet ../../PowerApps-TestEngine/bin/Debug/PowerAppsTestEngine/PowerAppsTestEngine.dll -i testPlan2.fx.yaml -p powerautomate -u local
```

## Backlog

A backlog of possible areas that could be investigated to improve unit test scenarios for the basic cloud flow sample:

- Validate properties of Actions. For example:
  - Validate that the trigger entity is **account** and the scope of the trigger (User, Business Unit or Organization)
  - Validate action evaluated value given values of outputs, variables and expressions.
- Validate the ability to evaluate expressions. For example `@triggerOutputs()?['body/name']`
- The ability to generate errors to that can Test `Scope:_Try` handling
- The ability to set variables before or after a action to allow for chaos testing
- Validate connection references used and types
