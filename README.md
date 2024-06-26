# Power Automate - Test Engine Module

This repository provides a proof of concept implementation of unit testing for Power Automate Cloud flows making use of extensions to the Power Apps Test Engine.

Key concepts demonstrated by this sample:

- Make use of extensibility model of Power Apps Test Engine to implement a Power Automate unit test provider

- Implement new Power FX actions that allow mocking of automation actions and triggers
  
- Validate the logic path of the cloud flow
  
- Assert the results of process and workflow results

## Prerequisites for building module

1. Install [.NET Core 6.0.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

2. Ensure that your `MSBuildSDKsPath` environment variable is pointing to [.NET Core 6.0.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

## Getting Started

1. Clone the repo and submodules

```bash
git clone --recurse-submodules https://github.com/Grant-Archibald-MS/PowerAutomate-TestEngineModule.git
```

2. Build the module

```bash
cd PowerAutomate-TestEngineModule\src
dotnet build
```

3. Run sample test

```bash
cd ..\samples\basic
dotnet ..\..\PowerApps-TestEngine\bin\Debug\PowerAppsTestEngine\PowerAppsTestEngine.dll -i testPlan2.fx.yaml -t 00000000-0000-0000-0000-000000000000 -e 00000000-0000-0000-0000-000000000000 -p powerautomate -u local
```

## Read More

Want to discover more about using Test Engine to test Power Automate Cloud flows? Our [README.md](./docs/README.md) provides more information.
