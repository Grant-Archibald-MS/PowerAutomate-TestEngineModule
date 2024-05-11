using Parser.FlowParser.ActionExecutors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testengine.provider.powerautomate
{

    public class PowerFxActionExecutor : ActionExecutorBase
    {
        public PowerFxActionExecutor()
        {

        }

        public PowerFxActionExecutor(ActionResult result)
        {
            Result = result;
        }

        /// <summary>
        /// The result that will be returned from <see cref="Execute"/>
        /// </summary>
        public ActionResult Result { get; set; } = new ActionResult() { ContinueExecution = true };

        public override Task<ActionResult> Execute()
        {
            return Task.FromResult(Result);
        }

        protected override void ProcessJson()
        {
            //TODO
        }
    }
}
