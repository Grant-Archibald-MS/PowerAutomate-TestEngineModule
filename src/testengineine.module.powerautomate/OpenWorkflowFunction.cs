using Microsoft.Extensions.Logging;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testengine.module
{
    public class OpenWorkflowFunction : ReflectionFunction
    {
        private readonly PowerAutomateTestState _state;
        private readonly ILogger _logger;

        public Func<string,string> GetFileContent = name => File.ReadAllText(name);

        public OpenWorkflowFunction(PowerAutomateTestState state, ILogger logger)
            : base("OpenWorkflow", FormulaType.String, FormulaType.Blank)
        {
            _state = state;
            _logger = logger;
        }

        public BlankValue Execute(StringValue source)
        {
            _logger.LogInformation("------------------------------\n\n" +
                "Executing OpenWorkflow function.");

            //
            _state.FlowRunner?.InitializeFlowRunner(new StringReader(GetFileContent(source.Value)));

            _logger.LogInformation("Successfully finished executing OpenWorkflow function.");

            return FormulaValue.NewBlank();
        }
    }
}
