using System.Collections.Generic;
using ShowCases.UAI;

public class Move: AgentAction
{
        public Move()
        {
                Description = "Moves closer to target, relative to movement speed";
                AddParameter("To Reload", true);
        }

        public override void OnStart(IAiContext context)
        {
                Act(context);
        }

        public override void OnGoing(IAiContext context)
        {
                Act(context);
        }

        private void Act(IAiContext context)
        {
                var archer = UAIHelper.GetArcherFromContext(context);
                var toReload = ParameterContainer.GetParamBool("To Reload").Value;
                archer.Move(toReload);
        }
}