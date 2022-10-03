using System.Collections.Generic;

namespace Services.Scorers.UtilityScorer
{
    public class USLowestScore: IUtilityScorer
    {
        private readonly string name = "Lowest Score";
        private readonly string description = "Returns the lowest score among the considerations";

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
            var lowest = float.MaxValue;
            foreach (var consideration in considerations)
            {
                var score = consideration.CalculateScore(context);
                if (score <= 0)
                {
                    return 0;
                }
                if(!consideration.IsScorer) continue;
                if (score < lowest)
                {
                    lowest = score;
                }
            }

            return lowest;
        }
    }
}