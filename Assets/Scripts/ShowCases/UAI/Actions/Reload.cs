using ShowCases.UAI;

public class Reload: AgentAction
{
        public Reload()
        {
                Description = "Moves to reload bench and reloads";
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
                var posX = archer.transform.localPosition.x;
                if (posX != 0)
                {
                        archer.Move(true);
                }
                else
                {
                        archer.Reload();
                }
        }
}