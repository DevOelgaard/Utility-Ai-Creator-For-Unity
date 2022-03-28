using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using System.Linq;
using MoreLinq;

[TestFixture]
public class UT_DecisionScoreEvaluator
{
    private DecisionScoreEvaluator uut;
    private List<Decision> mock_Decisions;
    private List<Bucket> mock_Buckets;
    private UtilityContainerSelector sub_DecisionSelector;
    private UtilityContainerSelector sub_BucketSelector;

    [SetUp]
    public void SetUp()
    {
        #region Init Mock Decisions
        var d1 = new Mock_Decision(0);
        var d2 = new Mock_Decision(5);
        var d3 = new Mock_Decision(-5);
        var d4 = new Mock_Decision(9.9f);
        var d5 = new Mock_Decision(10.0f);
        var d6 = new Mock_Decision(9.8f);

        mock_Decisions = new List<Decision>();
        mock_Decisions.Add(d1);
        mock_Decisions.Add(d2);
        mock_Decisions.Add(d3);
        mock_Decisions.Add(d4);
        mock_Decisions.Add(d5);
        mock_Decisions.Add(d6);

        #endregion

        #region Init Mock Buckets
        var b1 = new Mock_Bucket(0);
        var b2 = new Mock_Bucket(5);
        var b3 = new Mock_Bucket(-5);
        var b4 = new Mock_Bucket(9.9f);
        var b5 = new Mock_Bucket(10.0f);
        var b6 = new Mock_Bucket(9.8f);

        mock_Buckets = new List<Bucket>();
        mock_Buckets.Add(b1);
        mock_Buckets.Add(b2);
        mock_Buckets.Add(b3);
        mock_Buckets.Add(b4);
        mock_Buckets.Add(b5);
        mock_Buckets.Add(b6);

        for(var i = 0; i < mock_Buckets.Count; i++)
        {
            mock_Buckets[i].Decisions.Add(mock_Decisions[i]);
        }
        #endregion

        sub_DecisionSelector = Substitute.For<UtilityContainerSelector>();
        sub_BucketSelector = Substitute.For<UtilityContainerSelector>();

        sub_DecisionSelector.GetBestUtilityContainer(Arg.Any<List<Decision>>(), default)
            .ReturnsForAnyArgs(mock_Decisions
                .MaxBy(d => d.LastCalculatedUtility));

        sub_BucketSelector.GetBestUtilityContainer(mock_Buckets, default)
            .ReturnsForAnyArgs(mock_Buckets
                .MaxBy(b => b.LastCalculatedUtility));

        throw new NotImplementedException("NextActions should accept Ai");

        //uut = new DecisionScoreEvaluator(sub_DecisionSelector,sub_BucketSelector);
    }

    [Test]
    public void GetName_Default_ReturnsCorrectName()
    {
        Assert.AreEqual(Consts.Name_DefaultDSE, uut.GetName());
    }

    [Test]
    public void GetDescription_Default_ReturnsCorrectDescription()
    {
        Assert.AreEqual(Consts.Description_DefaultDSE, uut.GetDescription());
    }

    [Test]
    public void NextActions_Decisions_ReturnsCorrectCollection()
    {
        var highest = new Mock_Decision(11);
        mock_Decisions.Add(highest);

        var someLow = new Mock_Decision(2.5f);
        mock_Decisions.Add(someLow);

        sub_DecisionSelector.GetBestUtilityContainer(mock_Decisions, default)
            .ReturnsForAnyArgs(highest);

        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Decisions, default);

        //Assert.AreEqual(highest.AgentActions.Values, result);
    }

    [Test]
    public void NextActions_DecisionsNoElements_ReturnsNull()
    {
        mock_Decisions = new List<Decision>();
        
        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Decisions, default);

        //Assert.AreEqual(null, result);
    }

    [TestCase(0)]
    [TestCase(-0.0000001f)]
    [TestCase(-1)]
    [TestCase(-9999999)]
    public void NextActions_BestDecionScoreZeroOrLower_ReturnsNull(float returnValue)
    {
        var failDecision = new Mock_Decision(returnValue);
        sub_DecisionSelector.GetBestUtilityContainer(mock_Decisions, default)
            .ReturnsForAnyArgs(failDecision);

        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Decisions, default);

        //Assert.AreEqual(null, result);
    }

    [Test]
    public void NextActions_BucketsNoElements_ReturnsNull()
    {
        mock_Buckets = new List<Bucket>();

        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Buckets, default);

        //Assert.AreEqual(null, result);
    }

    [Test]
    public void NextActions_BucketsOneElement_ReturnsCorrectCollection()
    {
        var highest = new Mock_Bucket(15.5f);
        var highDecision = new Mock_Decision(222f);
        highest.Decisions.Add(highDecision);
        foreach (var d in mock_Decisions)
        {
            highest.Decisions.Add(d);
        }
        mock_Buckets = new List<Bucket>();
        mock_Buckets.Add(highest);

        sub_DecisionSelector
            .GetBestUtilityContainer(Arg.Is<List<Decision>>(l => l.Count > 6), default)
            .Returns(highDecision);

        sub_BucketSelector
            .GetBestUtilityContainer(mock_Buckets, default)
            .Returns(highest);
        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Buckets, default);

        //Assert.AreEqual(highDecision.AgentActions.Values, result);
    }

    [Test]
    public void NextActions_BucketsCorrectValues_ReturnsCorrectCollection()
    {
        var highest = new Mock_Bucket(15.5f);
        var highDecision = new Mock_Decision(222f);
        highest.Decisions.Add(highDecision);
        foreach(var d in mock_Decisions)
        {
            highest.Decisions.Add(d);
        }
        mock_Buckets.Add(highest);

        sub_DecisionSelector
            .GetBestUtilityContainer(Arg.Is<List<Decision>>(l => l.Count > 6), default)
            .Returns(highDecision);

        sub_BucketSelector
            .GetBestUtilityContainer(mock_Buckets, default)
            .Returns(highest);
        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Buckets, default);

        //Assert.AreEqual(highDecision.AgentActions.Values, result);
    }

    [Test]
    public void NextActions_HighestScoredBucketHasNoValidDecisions_ReturnsActionsFromSecondBucket()
    {
        var highValue = 15.5f;
        var lowValue = 0.1f;
        var invalidValue = 0f;

        mock_Buckets = new List<Bucket>();
        var highBucket = new Mock_Bucket(highValue);
        var invalidDecision = new Mock_Decision(invalidValue);
        highBucket.Decisions.Add(invalidDecision);
        mock_Buckets.Add(highBucket);

        var lowBucket = new Mock_Bucket(lowValue);
        var validDecision = new Mock_Decision(highValue);
        lowBucket.Decisions.Add(validDecision);
        mock_Buckets.Add(lowBucket);

        sub_DecisionSelector
            .GetBestUtilityContainer(Arg.Is<List<Decision>>(l => 
                l.Where(e => e.LastCalculatedUtility == invalidValue)
                .ToList()
                .Count() > 0), default)
            .Returns(invalidDecision);

        sub_DecisionSelector
            .GetBestUtilityContainer(Arg.Is<List<Decision>>(l =>
                l.Where(e => e.LastCalculatedUtility != invalidValue)
                .ToList()
                .Count() > 0), default)
            .Returns(validDecision);

        sub_BucketSelector
            .GetBestUtilityContainer(mock_Buckets, default)
            .Returns(highBucket);
        throw new NotImplementedException("NextActions should accept Ai");

        //var result = uut.NextActions(mock_Buckets, default);

        //Assert.AreEqual(validDecision.AgentActions.Values, result);
    }
}