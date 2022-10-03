namespace ShowCases.UAI
{
    public class UAIHelper
    {
        public static Archer GetArcherFromContext(IAiContext context)
        {
            var agent = context.Agent as AgentMono;
            return agent.gameObject.GetComponent<Archer>();
        }
    }
}