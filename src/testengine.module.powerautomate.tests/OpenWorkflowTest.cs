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

public class OpenWorkflowTest
{

    [Theory]
    [InlineData("foo.json", "{}")]
    public void CanSetup(string name, string json)
    {
        // Arrange
        var state = new PowerAutomateTestState();
        var MockLogger = new Mock<ILogger>();

        var action = new MockActionFunction(state, MockLogger.Object);
        var open = new OpenWorkflowFunction(state, MockLogger.Object);
        open.GetFileContent = (filename) => json;

        // Act
        open.Execute(FormulaValue.New(name));
  
        // Assert
    }
}