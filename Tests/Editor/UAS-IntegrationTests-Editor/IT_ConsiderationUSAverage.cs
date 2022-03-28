using System.Collections.Generic;
using NUnit.Framework;
using System;


[TestFixture]
public class IT_01ConsiderationUSAverage
{
    private USAverageScorer uut;

    [SetUp]
    public void SetUp()
    {
        uut = new USAverageScorer();
    }

    [TestCase(1, 1)]
    [TestCase(0.5f, 15)]
    [TestCase(0.8f, 15)]
    [TestCase(0.9f, 15)]
    public void CalculateUtility_MultipleSameScores_ReturnsInput(float calculatedScore, int numberOfConsiderations)
    {
        var considerations = CreateUniformConsiderations(numberOfConsiderations, calculatedScore);

        var result = uut.CalculateUtility(considerations, default);

        Assert.AreEqual(Math.Round(calculatedScore, 2), Math.Round(result, 2));
    }



    [Test]
    public void CalculateUtility_OneIsZero_ReturnsZero()
    {
        var considerations = CreateUniformConsiderations(10, 0.5f);
        var zeroConsideration = new Stub_Consideration_IT(0,new List<Parameter>());
        zeroConsideration.ReturnValue = 0;
        considerations.Add(zeroConsideration);

        var result = uut.CalculateUtility(considerations, default);

        Assert.AreEqual(0, result);
    }

    [Test]
    public void CalculateUtility_ValuesBelovZero_ReturnsZeroOrLower()
    {
        var considerations = CreateUniformConsiderations(10, -2);

        var result = uut.CalculateUtility(considerations, default);

        Assert.That(result <= 0);
    }

    [Test]
    public void CalculateUtility_NoConsiderations_ReturnsZeroOrLower()
    {
        var considerations = new List<Consideration>();

        var result = uut.CalculateUtility(considerations, default);

        Assert.That(result <= 0);
    }

    [TestCase(0.1f, 0.2f, 0.3f, 0.2f)]
    [TestCase(0.2f, 0.4f, 0.6f, 0.4f)]
    [TestCase(0.1f, 0.5f, 0.9f, 0.5f)]
    [TestCase(1f, 0.1f, 0.1f, 0.4f)]
    public void CalculateUtility_DifferentInput_ReturnsExpected(float a, float b, float c, float expected)
    {
        var considerations = new List<Consideration>();
        var m1 = new Stub_Consideration_IT(a, new List<Parameter>());
        var m2 = new Stub_Consideration_IT(b, new List<Parameter>());
        var m3 = new Stub_Consideration_IT(c, new List<Parameter>());

        considerations.Add(m1);
        considerations.Add(m2);
        considerations.Add(m3);

        var result = uut.CalculateUtility(considerations, default);

        Assert.AreEqual(expected, result);
    }



    private List<Consideration> CreateUniformConsiderations(int numberOfConsiderations, float returnValue, float min = 0f, float max = 1f)
    {
        var result = new List<Consideration>();
        for (var i = 0; i < numberOfConsiderations; i++)
        {
            var consideration = new Stub_Consideration_IT(returnValue, new List<Parameter>());
            consideration.MinFloat.Value = min;
            consideration.MaxFloat.Value = max;
            result.Add(consideration);
        }
        return result;
    }
}
