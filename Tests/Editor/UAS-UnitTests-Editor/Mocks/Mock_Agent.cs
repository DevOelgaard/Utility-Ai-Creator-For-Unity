namespace Mocks
{
    public class Mock_Agent: IAgent
    {
        public AgentModel Model { get; } = new AgentModel();

        public int TickCalls = 0;
        public Uai CurrentUai;
        public bool CanAutoTickBool;
        public string TypeIdentifierString { private get; set; }

        public string TypeIdentifier => string.IsNullOrEmpty(TypeIdentifierString) ? GetType().FullName : TypeIdentifierString;

        public void Tick(TickMetaData metaData)
        {
            TickCalls++;
        }

        public void SetAi(Uai newUai)
        {
            Uai = newUai;
        }

        public Uai Uai { get; set; }
        public bool CanAutoTick()
        {
            return CanAutoTickBool;
        }
    }
}