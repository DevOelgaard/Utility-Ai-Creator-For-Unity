using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

[TestFixture]
public class IT_ConsiderationUsAverageAiContextDecisionBucketUcsHighest
{
    private UCSHighestScore ucsHighestScore;
    private USAverageScorer uSAverage;
    private AiContext aIContext;
    private List<Bucket> buckets;
    private List<Decision> decisions;
    private int numberOfBuckets = 3;
    private int numberOfDecisions = 4;
    private int numberOfConsiderations = 5;


    [SetUp]
    public void SetUp()
    {
        ucsHighestScore = new UCSHighestScore();
        uSAverage = new USAverageScorer();
        aIContext = new AiContext();
        aIContext.UtilityScorer = uSAverage;
        buckets = CreateBucketsWithDecisions(numberOfBuckets, numberOfDecisions, numberOfConsiderations);
        decisions = CreateDecisionsWithConsiderations(numberOfDecisions, numberOfConsiderations);
    }

    [Test]
    public void GetBestUtilityContainer_BucketOneScoreHigher_ReturnsHighestScore()
    {
        var highestBucket = CreateBucketsWithDecisions(1, 2, 2, 0, 1, 1)[0];
        var lowBucket = CreateBucketsWithDecisions(1, 2, 2, 0, 1, 0.1f)[0];

        buckets.Add(highestBucket);
        buckets.Add(lowBucket);

        var result = ucsHighestScore.GetBestUtilityContainer(buckets, aIContext);

        Assert.AreEqual(highestBucket, result);
    }

    [Test]
    public void GetBestUtilityContainer_DecisionOneScoreHigher_ReturnsHighestScore()
    {
        var highestDecision = CreateDecisionsWithConsiderations(1, 2, 0, 1, 1)[0];
        var lowDecision = CreateDecisionsWithConsiderations(1, 2, 0, 1, 0.1f)[0];

        decisions.Add(highestDecision);
        decisions.Add(lowDecision);

        var result = ucsHighestScore.GetBestUtilityContainer(decisions, aIContext);

        Assert.AreEqual(highestDecision, result);
    }

    private List<Consideration> CreateConsiderations(int numberOfConsiderations, float min = 0f, float max = 1f, float returnValue = -1f)
    {
        var result = new List<Consideration>();
        for (var i = 0; i < numberOfConsiderations; i++)
        {
            if (returnValue < 0)
            {
                returnValue = (i / 100) * max;

            } 
            var consideration = new Stub_Consideration_IT(returnValue, new List<Parameter>());
            consideration.MinFloat.Value = min;
            consideration.MaxFloat.Value = max;
            result.Add(consideration);
        }
        return result;
    }

    private List<Decision> CreateDecisionsWithConsiderations(int numberOfDecisions, int numberOfConsiderations, float min = 0f, float max = 1f, float returnValue = -1f)
    {
        var result = new List<Decision>();
        for (var i = 0; i < numberOfDecisions; i++)
        {
            var decision = new Decision();
            result.Add(decision);
            var considerations = CreateConsiderations(numberOfConsiderations, min, max, returnValue);
            foreach(var c in considerations)
            {
                decision.Considerations.Add(c);
            }
        }
        return result;
    }

    private List<Bucket> CreateBucketsWithDecisions(int numberOfBuckets, int numberOfDecisions, int numberOfConsiderations, float min = 0f, float max = 1f, float returnValue = -1f)
    {
        var result = new List<Bucket>();
        for(var i = 0; i < numberOfBuckets; i++)
        {
            var b = new Bucket();
            result.Add(b);
            var decisions = CreateDecisionsWithConsiderations(numberOfDecisions, numberOfConsiderations, min, max, returnValue);
            foreach (var d in decisions)
            {
                b.Decisions.Add(d);
            }

            var bucketConsiderations = CreateConsiderations(numberOfConsiderations, min, max, returnValue);
            foreach(var c in bucketConsiderations)
            {
                b.Considerations.Add(c);
            }
        }
        return result;
    }
}
