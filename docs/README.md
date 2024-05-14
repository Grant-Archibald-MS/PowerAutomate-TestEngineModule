# Overview

The Power Apps Test Engine provider for Power Automate enables you to perform unit testing of Power Automate Cloud flow workflows.

## Architecture

![Power Automate Architecture diagram applied to Unit testing of cloud flows using extensions](./media/TestEngineOverview-PowerAutomate.svg)

## User Authentication

### Local Loop Testing

The local authentication use useful the definition of the cloud flow is locally available on the local computer. In this use case no authentication is required for unit tests.

### Integration Testing

To run a integration test user authentication may be needed to trigger test of the cloud flow from the Power Automate portal

## Power Automate Provider

Create a new provider that makes use of knowledge of the structure of the work flow to create Power Fx representation of a Workflow. This enables Power FX variables to be created that can query and update of  to manage the test state.

## Power Automate Extensions

You can read more on the specific [PowerFX](./PowerFX/README.md) that enable unit test of Power Automate Cloud flows.

## Test Persona

Looking at different testing persona and how these test extensions apply

|Persona|Description|Notes|
|-------|-----------|-----|
|Code first developer|A maker who is familiar working with code editors and local execution | Initial target user type |
|Reviewer | A peer reviewer or architecture / development lead role reviewing the development work | Interested in test coverage and metrics |
| Low code maker | A low code maker building business applications. Not familiar with working code editors and yaml | Not initial audience of this project. Could be assisted in future with Generative AI summarization and test case management. |
| Support Engineers | Use of tests to isolate and validate fixes for issues | Can use tests to ensure business continuity of high value automation solutions |
