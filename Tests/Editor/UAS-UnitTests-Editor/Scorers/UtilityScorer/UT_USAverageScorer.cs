using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;

[TestFixture]
public class UT_USAverageScorer
{
    private USAverageScorer uut;

    private List<Consideration> mock_Considerations;

    [SetUp]
    public void SetUp()
    {
        uut = new USAverageScorer();

        mock_Considerations = new List<Consideration>();
    }

    [Test]
    public void Constructor_Default_ReturnsObject()
    {
        Assert.That(uut != null);
    }

    [Test]
    public void GetName_Default_ReturnsCorrectName()
    {
        Assert.AreEqual(Consts.Name_USAverageScore, uut.GetName());
    }

    [Test]
    public void GetDescription_Default_ReturnsCorrectDescription()
    {
        Assert.AreEqual(Consts.Description_USAverageScore, uut.GetDescription());
    }

    [TestCase(1,1)]
    [TestCase(0.5f,15)]
    [TestCase(0.8f,15)]
    [TestCase(0.9f,15)]
    public void CalculateUtility_MultipleSameScores_ReturnsInput(float calculatedScore, int numberOfConsiderations)
    {
        mock_Considerations = GetConsiderationMocks(numberOfConsiderations, calculatedScore);

        var result = uut.CalculateUtility(mock_Considerations, default);

        Assert.AreEqual(Math.Round(calculatedScore,2),Math.Round(result,2));
    }

    [Test]
    public void CalculateUtility_OneIsZero_ReturnsZero()
    {
        mock_Considerations = GetConsiderationMocks(10, 0.5f);
        var zeroMock = new Mock_ConsiderationSimple();
        zeroMock.ReturnValue = 0;
        mock_Considerations.Add(zeroMock);

        var result = uut.CalculateUtility(mock_Considerations, default);

        Assert.AreEqual(0, result);
    }

    [Test]
    public void CalculateUtility_ValuesBelovZero_ReturnsZeroOrLower()
    {
        mock_Considerations = GetConsiderationMocks(10, -2);
        
        var result = uut.CalculateUtility(mock_Considerations, default);

        Assert.That(result <= 0);
    }

    [Test]
    public void CalculateUtility_NoConsiderations_ReturnsZeroOrLower()
    {
        mock_Considerations = new List<Consideration>();

        var result = uut.CalculateUtility(mock_Considerations, default);

        Assert.That(result <= 0);
    }

    [TestCase(1,2,3,2)]
    [TestCase(2,4,6,4)]
    [TestCase(1,5,9,5)]
    [TestCase(10,1,1,4)]
    public void CalculateUtility_DifferentInput_ReturnsExpected(float a, float b, float c, float expected)
    {
        mock_Considerations = new List<Consideration>();
        var m1 = new Mock_ConsiderationSimple();
        var m2 = new Mock_ConsiderationSimple();
        var m3 = new Mock_ConsiderationSimple();
        
        m1.ReturnValue = a;
        m2.ReturnValue = b;
        m3.ReturnValue = c;

        mock_Considerations.Add(m1);
        mock_Considerations.Add(m2);
        mock_Considerations.Add(m3);

        var result = uut.CalculateUtility(mock_Considerations, default);

        Assert.AreEqual(expected, result);
    }

    private List<Consideration> GetConsiderationMocks(int numberOfConsiderations, float returnValue)
    {
        var result = new List<Consideration>();
        for (var i = 0; i < numberOfConsiderations; i++)
        {
            var mock = new Mock_ConsiderationSimple();
            mock.ReturnValue = returnValue;
            result.Add(mock);
        }
        return result;
    }

}
