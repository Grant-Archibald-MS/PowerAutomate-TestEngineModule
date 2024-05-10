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

public class MockActionTest
{

    [Theory]
    [InlineData("Name", false)]
    [InlineData("Name", true)]
    public void CanSetup(string name, bool continueExecution)
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var build = new BuildTestFunction(state, MockLogger.Object);

        // Act
        action.Execute(FormulaValue.New(name), FormulaValue.New(continueExecution));
        build.Execute();

        // Assert
        var match = state.Provider?.GetServices<ActionExecutorRegistration>().Where(e => e.ActionName == name).FirstOrDefault();
        Assert.NotNull(match);
        Assert.IsType<PowerFxActionExecutor>(match.Instance);
        var instance = match.Instance as PowerFxActionExecutor;
        Assert.Equal(continueExecution, instance?.Result.ContinueExecution);
    }
}