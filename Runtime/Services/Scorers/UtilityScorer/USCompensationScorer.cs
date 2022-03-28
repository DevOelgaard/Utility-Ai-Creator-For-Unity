using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class USCompensationScorer : IUtilityScorer
{


    public float CalculateUtility(List<Consideration> considerations, AiContext context)
    {
        var modificationFactor = 1 - (1 / considerations.Count);
        var utility = 1f;

        foreach(var consideration in considerations)
        {
            var score = consideration.CalculateScore(context);
            var makeUpValue = (1 - score) * modificationFactor;
            var finalScore = score + (makeUpValue * score);
            utility *= finalScore;
            if (utility == 0)
            {
                return 0;
            }
        }
        return utility;
    }

    public string GetDescription()
    {
        return Consts.Description_USCompensationScorer;
    }

    public string GetName()
    {
        return Consts.Name_USCompensationScorer;
    }
}
