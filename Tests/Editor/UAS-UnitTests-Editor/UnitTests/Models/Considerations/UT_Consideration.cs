using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using UniRx;
using System.ComponentModel;
using Mocks;

namespace UnitTests.Models.Considerations
{
    [TestFixture]
    public class UT_Consideration
    {
        private Stub_Consideration_UT uut;

        [SetUp]
        public void SetUp()
        {
            uut = new Stub_Consideration_UT(0.5f, new List<ParamBase>());
            uut.MinFloat.Value = 0f;
            uut.MaxFloat.Value = 1f;
        }

        [Test]
        public void GetTypeDescription_NoInput_ReturnsCorrectString()
        {
            var expected =uut.GetType().ToString();
            var result = uut.GetTypeDescription();

            Assert.AreEqual(expected, result);
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
        public void Clone_NewObject_ReturnsClone()
        {
            var result = uut.Clone();
            
            Assert.AreEqual(uut.Name,result.Name);
            Assert.AreEqual(uut.Description,result.Description);
        }

        [TestCase(-1, 0)]
        [TestCase(0, 0)]
        [TestCase(0.1f, 0.1f)]
        [TestCase(0.5f, 0.5f)]
        [TestCase(0.9f, 0.9f)]
        [TestCase(1f, 1f)]
        [TestCase(1.1f, 1f)]
        public void CalculateScore_EdgeCasesAroundMinMax_ReturnsExpected(float baseScore, float expected)
        {

            uut.ReturnValue = baseScore;

            var result = uut.CalculateScore(default);

            Assert.AreEqual((float) Math.Round(expected, 2), (float) Math.Round(result, 2));
        }
    }
}