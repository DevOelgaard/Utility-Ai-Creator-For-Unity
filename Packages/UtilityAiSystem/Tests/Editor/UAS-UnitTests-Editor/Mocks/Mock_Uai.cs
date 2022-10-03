using System.Runtime.Remoting.Contexts;

namespace Mocks
{
    public class Mock_Uai: Uai
    {
        public Mock_Uai()
        {
            UaiContext.TickMetaData = new TickMetaData();
        }
    }
}