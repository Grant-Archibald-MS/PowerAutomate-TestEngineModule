using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerApps.TestEngine.Modules;
using Microsoft.PowerFx.Types;
using Mono.Cecil;
using Moq;
using Parser.ExpressionParser;
using Parser.FlowParser.ActionExecutors;
using testengine.module;

namespace testengine.provider.powerautomate.tests;

public class BuildTest
{
    private static readonly string TestFlowPath = System.IO.Path.GetFullPath(@"..\..\..\..\..\PowerAutomateMockUp\Test\FlowSamples");
    private string SampleFlowPath;

    public BuildTest()
    {
        SampleFlowPath = @$"{TestFlowPath}\PowerAutomateMockUpSampleFlow.json";
    }

    [Fact]
    public void CanBuildFlowRunnerService()
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var MockLogger = new Mock<ILogger>();
        var action = new BuildTestFunction(state, MockLogger.Object);

        // Act
        action.Execute();

        // Assert
        Assert.NotNull(state.FlowRunner);
    }

    [Fact]
    public void SampleFlowTriggerIsFaulted()
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var MockLogger = new Mock<ILogger>();
        var action = new BuildTestFunction(state, MockLogger.Object);

        // Act
        action.Execute();
        state.FlowRunner?.InitializeFlowRunner(new StringReader(File.ReadAllText(SampleFlowPath)));
        var result = state.FlowRunner?.Trigger();

        // Assert
        Assert.Equal(TaskStatus.Faulted, result?.Status);
    }

    [Fact]
    public void SampleFlowTriggerIsCompleted()
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var build = new BuildTestFunction(state, MockLogger.Object);
        var open = new OpenWorkflowFunction(state, MockLogger.Object);
        var trigger = new TriggerWorkflowFunction(state, MockLogger.Object);

        // Act
        action.Execute(FormulaValue.New("Get_a_record_-_Valid_Id"), FormulaValue.New(true));
        action.Execute(FormulaValue.New("Update_Account_-_Invalid_Id"), FormulaValue.New(true));
        build.Execute();
        open.Execute(FormulaValue.New(SampleFlowPath));
        var result = trigger.Execute(FormulaValue.New("{'body':{ 'name':'Alice Bob', 'accountid': 1 }}"));

        // Assert
 
    }
}