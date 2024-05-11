using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Microsoft.PowerApps.TestEngine.Modules;
using Microsoft.PowerFx.Types;
using Mono.Cecil;
using Moq;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using Parser;
using Parser.ExpressionParser;
using Parser.FlowParser;
using Parser.FlowParser.ActionExecutors;
using System.Runtime.InteropServices;
using System.Text.Json;
using testengine.module;

namespace testengine.provider.powerautomate.tests;

public class TriggerWorkflowTest
{
    private string SampleWorkFlow = "";
    public TriggerWorkflowTest() {
        var sampleFile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),"..", "..", "..", "..", "..", "samples", "basic", "PowerAutomateMockUpSampleFlow.json");
        SampleWorkFlow = File.ReadAllText(sampleFile);
    }


    [Theory]
    [InlineData("{'test':'A'}", 2, "test")]
    [InlineData("{'body':{'name':'test','accountid':1}}", 2, "body/name,body/accountid")]
    public void StartTrigger(string json, double expectedActionCount, string expectedOutputs)
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var parserState = new State();
        var mockLogger = new Mock<ILogger<ScopeDepthManager>>();
        var mockFlowRunnerLogger = new Mock<ILogger<FlowRunner>>();
        var scopeDepthManager = new ScopeDepthManager(mockLogger.Object);
        var mockSettings = new Mock<IOptions<FlowSettings>>();
        var mockExecutor = new Mock<IActionExecutorFactory>();
        var mockExpressionEngine = new Mock<IExpressionEngine>();
        var flowSettings = new FlowSettings() { FailOnUnknownAction = false };

        mockSettings.Setup(x => x.Value).Returns(flowSettings);

        state.FlowRunner = new FlowRunner(
            parserState,
            scopeDepthManager,
            mockSettings.Object,
            mockExecutor.Object,
            mockFlowRunnerLogger.Object,
            mockExpressionEngine.Object);

        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var trigger = new TriggerWorkflowFunction(state, MockLogger.Object);


        // Act
        state.FlowRunner.InitializeFlowRunner(new StringReader(SampleWorkFlow));
        var result = trigger.Execute(FormulaValue.New(json));

        // Assert
        Assert.Equal(expectedActionCount, result.GetField("NumberOfExecutedActions").AsDouble());
        var outputs = parserState.GetTriggerOutputs();

        var outputData = outputs.AsDict();
        AssertOutputs(expectedOutputs, outputData);
    }

    private void AssertOutputs(string expected, Dictionary<string, ValueContainer> outputData)
    {
        foreach ( var item in expected.Split(',') ) { 
            if ( item.Contains("/") )
            {
                var parts = item.Split("/");
                Assert.True(FindMatch(parts, 0, outputData), $"Did not find {item}");
            } else
            {
                Assert.True(outputData.ContainsKey(item),$"Did not find {item}");
            }
        }
    }

    private bool FindMatch(string[] parts, int index, Dictionary<string, ValueContainer> outputData)
    {
        if (outputData.ContainsKey(parts[index]) )
        {
            if ( index < parts.Length - 1 )
            {
                // We have successfully found the requested key, we can end the search
                return true;
            }

            // Keey searching
            var child = outputData[parts[index]];
            if ( child.Type() == ValueContainer.ValueType.Object)
            {
                // Search child object
                var childValue = child.GetValue<Dictionary<string, ValueContainer>>();
                return FindMatch(parts, index + 1, childValue);
            }
            return false;
        }
        return false;
    }
}