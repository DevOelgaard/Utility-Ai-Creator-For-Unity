using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class IT_ConsiderationUsAverageAiContextDecision
{
    private Decision decision;
    private USAverageScorer uSAverage;
    private AiContext aIContext;
    
    
    [SetUp]
    public void SetUp()
    {
        decision = new Decision();
        uSAverage = new USAverageScorer();
        aIContext = new AiContext();
        aIContext.UtilityScorer = uSAverage;
    }

    [TestCase(0,10)]
    public void GetUtility_CorrectValues_ReturnsExpected(float calculatedScore, int numberOfConsiderations)
    {
        var considerations = CreateUniformConsiderations(numberOfConsiderations, calculatedScore);
        
        decision.Considerations.Add(considerations);
        //foreach(var c in considerations)
        //{
        //    decision.Considerations.Add(c);
        //}

        var result = decision.GetUtility(aIContext);

        Assert.AreEqual(Math.Round(calculatedScore, 2), Math.Round(result, 2));
    }

    [TestCase(0, 10, 0, 0)]
    [TestCase(0.9f, 0.5f, 0.1f, 0.5f)]
    [TestCase(0.3f, 0.5f, 0.4f, 0.4f)]
    [TestCase(-1, 0.9f, 1, 0)]
    [TestCase(1, 1, -1, 0)]
    [TestCase(2, 1, -1, 0)]
    public void GetUtility_MultipleValues_ReturnsExpected(float a, float b, float c, float expected)
    {
        var c1 = new Stub_Consideration_IT(a, new List<Parameter>());
        var c2 = new Stub_Consideration_IT(b, new List<Parameter>());
        var c3 = new Stub_Consideration_IT(c, new List<Parameter>());
        decision.Considerations.Add(c1);
        decision.Considerations.Add(c2);
        decision.Considerations.Add(c3);

        var result = decision.GetUtility(aIContext);

        Assert.AreEqual(Math.Round(expected, 2), Math.Round(result, 2));
    }

    [TestCase(1,1,2,0,8,0.5f)]
    [TestCase(3,-1,1,0,10,0.3f)]
    [TestCase(-1,-2,-3,-10,0,0.4f)]
    [TestCase(30,15,10,20,120,0.35f)]
    public void GetUtility_ConsiderationParameters_ReturnsExpected( float p1, float p2, float p3, float min, float max, float expected)
    {
        var considerations = CreateUniformConsiderations(5, 0);


        foreach(var consideration in considerations)
        {
            consideration.Parameters.Add(new Parameter("p1",p1));
            consideration.Parameters.Add(new Parameter("p2",p2));
            consideration.Parameters.Add(new Parameter("p3",p3));

            consideration.MinFloat.Value = min;
            consideration.MaxFloat.Value = max;

            decision.Considerations.Add(consideration);
        }

        var result = decision.GetUtility(aIContext);

        Assert.AreEqual(Math.Round(expected, 2), Math.Round(result, 2));
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
