using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UsAverageScorer : IUtilityScorer
{
    private readonly string name = Consts.Name_USAverageScore;
    private readonly string description = Consts.Description_USAverageScore;
    public float CalculateUtility(List<Consideration> considerations, IAiContext context)
    {
        if (considerations.Count == 0)
            return 0;

        var sum = 0f;
        var amountOfScorers = 0;
        foreach (var consideration in considerations.Where(c => !c.IsModifier))
        {
            var score = consideration.CalculateScore(context);
            
            // If any consideration fails, the decision/bucket can't be executed
            if (score <= 0)
            {
                return score;
            }
            if (consideration.IsScorer)
            {
                amountOfScorers++;
                sum += score;
            }
        }
        if(amountOfScorers <= 0) // Only Considerations are Bools or Modifiers have been calculated. If they failed they would have returned false
        {
            return 1;
        } else
        {
            return (sum / amountOfScorers);
        }

    }

    public string GetDescription() => description;

    public string GetName() => name;
}
