using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerApps.TestEngine.Modules;
using Microsoft.PowerFx.Types;
using Mono.Cecil;
using Moq;
using NuGet.Configuration;
using Parser;
using Parser.ExpressionParser;
using Parser.FlowParser;
using Parser.FlowParser.ActionExecutors;
using System.Runtime.InteropServices;
using testengine.module;

namespace testengine.provider.powerautomate.tests;

public class TriggerWorkflowTest
{

    [Theory]
    [InlineData("StringValue","A")]
    public void TriggerWithSimpleRecord(string name, object value)
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var parserState = new State();
        var mockScopeDepthManager = new Mock<IScopeDepthManager>();
        var mockSettings = new Mock<IOptions<FlowSettings>>();
        var mockExecutor = new Mock<IActionExecutorFactory>();
        var mockLogger = new Mock<ILogger<FlowRunner>>();
        var mockExpressionEngine = new Mock<IExpressionEngine>();

        state.FlowRunner = new FlowRunner(
            parserState,
            mockScopeDepthManager.Object,
            mockSettings.Object,
            mockExecutor.Object,
            mockLogger.Object,
            mockExpressionEngine.Object);

        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var open = new TriggerWorkflowFunction(state, MockLogger.Object);

        NamedValue namedValue = null;
        switch (value.GetType().ToString())
        {
            case "System.String":
                namedValue = new NamedValue(name, FormulaValue.New((string)value));
                break;
        }
        var input = RecordValue.NewRecordFromFields(namedValue);

        // Act
        open.Execute(input);

        // Assert
        var results = parserState.GetTriggerOutputs();
        var data = results.GetValue<Dictionary<string, ValueContainer>>();
        var getValue = data[name].GetType().GetMethod("GetValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var genericGetValue = getValue?.MakeGenericMethod(value.GetType());
        var triggerValue = genericGetValue?.Invoke(data[name], null);

        Assert.Equal(value, triggerValue);
    }

    [Theory]
    [InlineData("StringValue", "A")]
    public void TriggerWithNestedRecord(string name, object value)
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var parserState = new State();
        var mockScopeDepthManager = new Mock<IScopeDepthManager>();
        var mockSettings = new Mock<IOptions<FlowSettings>>();
        var mockExecutor = new Mock<IActionExecutorFactory>();
        var mockLogger = new Mock<ILogger<FlowRunner>>();
        var mockExpressionEngine = new Mock<IExpressionEngine>();

        state.FlowRunner = new FlowRunner(
            parserState,
            mockScopeDepthManager.Object,
            mockSettings.Object,
            mockExecutor.Object,
            mockLogger.Object,
            mockExpressionEngine.Object);

        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var open = new TriggerWorkflowFunction(state, MockLogger.Object);



        NamedValue namedValue = null;
        switch (value.GetType().ToString())
        {
            case "System.String":
                namedValue = new NamedValue(name, FormulaValue.New((string)value));
                break;
        }


        var child = RecordValue.NewRecordFromFields(namedValue);
        var input = RecordValue.NewRecordFromFields(new NamedValue("wrapper", child));

        // Act
        open.Execute(input);

        // Assert
        var results = parserState.GetTriggerOutputs();
        var data = results.GetValue<Dictionary<string, ValueContainer>>();
        var getValue = data["wrapper"].GetType().GetMethod("GetValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        var genericGetValue = getValue?.MakeGenericMethod(typeof(ValueContainer));
        var triggerContainerValue = genericGetValue?.Invoke(data["wrapper"], null);

        Assert.NotNull(triggerContainerValue);
    }
}