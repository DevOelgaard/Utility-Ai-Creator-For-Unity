using ShowCases.UAI;

public class Shoot: AgentAction
{
        public Shoot()
        {
                Description = "Shoots at target";
                HelpText = "Must have ammo";
        }

        public override void OnStart(IAiContext context)
        {
                ShootAtTarget(context);
        }

        public override void OnGoing(IAiContext context)
        {
                ShootAtTarget(context);
        }

        public override void OnEnd(IAiContext context)
        {
                base.OnEnd(context);
        }


        private void ShootAtTarget(IAiContext context)
        {
                var archer = UAIHelper.GetArcherFromContext(context);
                archer.ShootTarget();
        }
}