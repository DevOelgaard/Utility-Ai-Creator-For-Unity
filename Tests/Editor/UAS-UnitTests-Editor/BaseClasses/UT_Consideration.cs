using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using UniRx;
using System.ComponentModel;

[TestFixture]
public class UT_Consideration
{
    private Stub_Consideration_UT uut;

    [SetUp]
    public void SetUp()
    {
        uut = new Stub_Consideration_UT(0.5f, new List<Parameter>());
        uut.MinFloat.Value = 0f;
        uut.MaxFloat.Value = 1f;
    }

    [Test]
    public void BaseScoreChanged_NewBaseScoreCalculated_PuplishesEventOneTime()
    {
        var timesPuplished = 0;
        var sub = uut
            .BaseScoreChanged
            .Subscribe(_ => timesPuplished++);

        uut.CalculateScore(default);

        sub.Dispose();
        Assert.AreEqual(1, timesPuplished);
    }

    [Test]
    public void NormalizedScoreChanged_NewNormalizedScoreCalculated_PuplishesEventOneTime()
    {
        var timesPuplished = 0;
        var sub = uut
            .NormalizedScoreChanged
            .Subscribe(_ => timesPuplished++);

        uut.CalculateScore(default);

        sub.Dispose();
        Assert.AreEqual(1, timesPuplished);
    }

    [Test]
    public void GetName_AfterInitialization_ReturnsNameWithCorrectPostFix()
    {
        Assert.That(uut.Name.Contains("(Stub_Consideration_UT)"));
    }

    [TestCase("TestName")]
    [TestCase("Name with spaces")]
    [TestCase("Name with 1 number")]
    [TestCase("Name with symbols !%")]
    public void GetName_AfterSettingName_ReturnsNameWithCorrectPostFix(string n)
    {
        uut.Name = n;
        var expected = n + " (Stub_Consideration_UT)";
        Assert.AreEqual(expected, uut.Name);
    }

    [TestCase(-1,0)]
    [TestCase(0,0)]
    [TestCase(0.1f,0.1f)]
    [TestCase(0.5f,0.5f)]
    [TestCase(0.9f,0.9f)]
    [TestCase(1f,1f)]
    [TestCase(1.1f,1f)]
    public void CalculateScore_EdgeCasesAroundMinMax_ReturnsExpected(float baseScore, float expected)
    {

        uut.ReturnValue = baseScore;
        
        var result = uut.CalculateScore(default);
        
        Assert.AreEqual((float)Math.Round(expected,2), (float)Math.Round(result,2));
    }

    [TestCase(2,4,3,0.5f)]
    [TestCase(-1,1,0,0.5f)]
    [TestCase(100,200,150,0.5f)]
    [TestCase(100,200,190,0.9f)]
    [TestCase(-10,-5,-7.5f,0.5f)]
    public void CalculateScore_WithCustomMinMax_ReturnsExpected(float min, float max, float baseScore, float expected)
    {
        uut.MinFloat.Value = min;
        uut.MaxFloat.Value = max;
        uut.ReturnValue = baseScore;

        var result = uut.CalculateScore(default);

        Assert.AreEqual((float)Math.Round(expected, 2), (float)Math.Round(result, 2));
    }
}
