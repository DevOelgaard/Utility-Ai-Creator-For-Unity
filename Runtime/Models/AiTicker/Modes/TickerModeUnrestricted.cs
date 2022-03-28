using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeUnrestricted : TickerMode
{
    public TickerModeUnrestricted() : base(AiTickerMode.Unrestricted, Consts.Description_TickerModeUnrestricted)
    {

    }

    internal override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        agents.ForEach(agent =>
        {
            agent.Tick(metaData);
        });
    }
}