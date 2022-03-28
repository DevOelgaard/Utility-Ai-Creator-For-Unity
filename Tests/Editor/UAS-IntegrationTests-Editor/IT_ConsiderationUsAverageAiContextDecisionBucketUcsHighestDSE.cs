using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]

public class IT_ConsiderationUsAverageAiContextDecisionBucketUcsHighestDSE
{
    private DecisionScoreEvaluator dSE;
    private UCSHighestScore ucsHighestScore;
    private USAverageScorer uSAverage;
    private AiContext aIContext;
    private List<Bucket> buckets;
    private int numberOfBuckets = 3;
    private int numberOfDecisions = 4;
    private int numberOfConsiderations = 5;

    [SetUp]
    public void SetUp()
    {
        ucsHighestScore = new UCSHighestScore();
        dSE = new DecisionScoreEvaluator();
        uSAverage = new USAverageScorer();
        aIContext = new AiContext();
        aIContext.UtilityScorer = uSAverage;
        buckets = CreateBucketsWithDecisions(numberOfBuckets, numberOfDecisions, numberOfConsiderations);
    }

    [Test]
    public void NextActions_Buckets_ReturnsActionsOfHighestScoredBucket()
    {
        var highestAction = new Mock_AgentAction();
        highestAction.Name = "Highest";

        var highConsideration = new Stub_Consideration_IT(1, new List<Parameter>());

        var highDecisions = new Decision();
        highDecisions.AgentActions.Add(highestAction);
        highDecisions.Considerations.Add(highConsideration);

        var highBucket = new Bucket();
        highBucket.Decisions.Add(highDecisions);
        highBucket.Considerations.Add(highConsideration);
        highBucket.Weight.Value = 9;

        buckets.Add(highBucket);

        // Adding low buckets to put high bucket in the middle of list
        var lowBuckets = CreateBucketsWithDecisions(2, 2, 2, 0, 1, 0.1f);
        foreach(var b in lowBuckets)
        {
            buckets.Add(b);
        }

        throw new NotImplementedException("NextActions should accept Ai");
        //var result = dSE.NextActions(buckets, aIContext);

        //Assert.AreEqual(highestAction, result[0]);
    }

    [Test]
    public void NextActions_HighBucketWeightWins_ReturnsBucketWithHighestScore()
    {
        var highestAction = new Mock_AgentAction();
        highestAction.Name = "Highest";

        var mediumConsideration = new Stub_Consideration_IT(0.5f, new List<Parameter>());

        var mediumDecision = new Decision();
        mediumDecision.AgentActions.Add(highestAction);
        mediumDecision.Considerations.Add(mediumConsideration);

        var highWeightBucket = new Bucket();
        highWeightBucket.Decisions.Add(mediumDecision);
        highWeightBucket.Considerations.Add(mediumConsideration);
        highWeightBucket.Weight.Value = 9;

        var loverAction = new Mock_AgentAction();
        highestAction.Name = "Lower";

        var highConsideration = new Stub_Consideration_IT(0.5f, new List<Parameter>());

        var highDecisions = new Decision();
        highDecisions.AgentActions.Add(loverAction);
        highDecisions.Considerations.Add(mediumConsideration);

        var highConsiderationBucket = new Bucket();
        highConsiderationBucket.Decisions.Add(highDecisions);
        highConsiderationBucket.Considerations.Add(highConsideration);
        highConsiderationBucket.Weight.Value = 2;

        buckets.Add(highWeightBucket);
        buckets.Add(highConsiderationBucket);

        throw new NotImplementedException("NextActions should accept Ai");

        //var result = dSE.NextActions(buckets, aIContext);

        //Assert.AreEqual(highestAction, result[0]);
    }



    private List<Consideration> CreateConsiderations(int numberOfConsiderations, float min = 0f, float max = 1f, float returnValue = -1f)
    {
        var result = new List<Consideration>();
        for (var i = 0; i < numberOfConsiderations; i++)
        {
            if (returnValue < 0)
            {
                returnValue = (i / 100) * max + 0.1f;

            }
            var consideration = new Stub_Consideration_IT(returnValue, new List<Parameter>());
            consideration.MinFloat.Value = min;
            consideration.MaxFloat.Value = max;
            result.Add(consideration);
        }
        return result;
    }

    private List<Decision> CreateDecisionsWithConsiderations(int numberOfDecisions, int numberOfConsiderations, string actionName = "", float min = 0f, float max = 1f, float returnValue = -1f)
    {
        var result = new List<Decision>();
        for (var i = 0; i < numberOfDecisions; i++)
        {
            var decision = new Decision();
            decision.Name = "D" + i;
            result.Add(decision);
            var considerations = CreateConsiderations(numberOfConsiderations, min, max, returnValue);
            foreach (var c in considerations)
            {
                decision.Considerations.Add(c);
            }

            var action = new Mock_AgentAction();
            action.Name = actionName + decision.Name + "A";
            decision.AgentActions.Add(action);
        }
        return result;
    }

    private List<Bucket> CreateBucketsWithDecisions(int numberOfBuckets, int numberOfDecisions, int numberOfConsiderations, float min = 0f, float max = 1f, float returnValue = -1f, float bucketWeight = 1f)
    {
        var result = new List<Bucket>();
        for (var i = 0; i < numberOfBuckets; i++)
        {
            var b = new Bucket();
            b.Name = "B" + i;
            b.Weight.Value = bucketWeight;
            result.Add(b);

            var decisions = CreateDecisionsWithConsiderations(numberOfDecisions, numberOfConsiderations, b.Name, min, max, returnValue);
            foreach (var d in decisions)
            {
                b.Decisions.Add(d);
            }

            var bucketConsiderations = CreateConsiderations(numberOfConsiderations, min, max, returnValue);
            foreach (var c in bucketConsiderations)
            {
                b.Considerations.Add(c);
            }
        }
        return result;
    }
}
