using System.Collections.Generic;

namespace Services.Scorers.UtilityScorer
{
    public class USHighestScore: IUtilityScorer
    {
        private readonly string name = "Highest Score";
        private readonly string description = "Returns the highest score among the considerations";

        public string GetName()
        {
            return name;
        }

        public string GetDescription()
        {
            return description;
        }

        public float CalculateUtility(List<Consideration> considerations, IAiContext context)
        {
            var highestScore = float.MinValue;
            foreach (var consideration in considerations)
            {
                var score = consideration.CalculateScore(context);
                if (score <= 0)
                {
                    return 0;
                }
                if(!consideration.IsScorer) continue;
                if (score > highestScore)
                {
                    highestScore = score;
                }
            }

            return highestScore;
        }
    }
}