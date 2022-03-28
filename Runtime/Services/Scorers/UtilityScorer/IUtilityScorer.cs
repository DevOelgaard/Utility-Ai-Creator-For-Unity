using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUtilityScorer: IIdentifier
{
    public float CalculateUtility(List<Consideration> considerations, AiContext context);
}
