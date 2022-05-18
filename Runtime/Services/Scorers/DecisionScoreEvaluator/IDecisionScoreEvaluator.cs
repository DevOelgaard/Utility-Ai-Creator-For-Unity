using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDecisionScoreEvaluator: IIdentifier
{
    public List<AgentAction> NextActions(List<Decision> decisions, Uai uai);
    public List<AgentAction> NextActions(List<Bucket> buckets, Uai uai);

}
