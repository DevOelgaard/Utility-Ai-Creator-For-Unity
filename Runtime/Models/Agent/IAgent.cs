using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAgent
{
    AgentModel Model { get; }
    string TypeIdentifier { get; }
    void Tick(TickMetaData metaData);
    void SetAi(Uai model);
    Uai Uai { get; set; }
    bool CanAutoTick();
}