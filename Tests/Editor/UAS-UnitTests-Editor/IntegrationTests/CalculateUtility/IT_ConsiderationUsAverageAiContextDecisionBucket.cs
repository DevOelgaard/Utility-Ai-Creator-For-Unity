using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;
using Mocks;
using UniRx;

namespace IntegrationTests.CalculateUtility
{
    [TestFixture]
    public class IT_ConsiderationUsAverageAiContextDecisionBucket
    {
        private Bucket bucket;
        private UsAverageScorer uSAverage;
        private IAiContext aIContext;

        [SetUp]
        public void SetUp()
        {
            bucket = new Bucket();
            bucket.Initialize();
            uSAverage = new UsAverageScorer();
            aIContext = new AiContext();
            aIContext.UtilityScorer = uSAverage;
        }

        [Test]
        public void GetConsiderations_AfterInitialization_ReturnsZeroConsideration()
        {
            Assert.AreEqual(0, bucket.Considerations.Values.Count());
        }

        [Test]
        public void LastUtilityChanged_CalculatingUtility_PublishesEvent()
        {
            var timesCalled = 0;
            var sub = bucket
                .LastUtilityScoreChanged
                .Subscribe(_ => timesCalled++);

            var consideration = new Stub_Consideration_IT(0.7f, new List<Parameter>());
            consideration.Initialize();
            bucket.Considerations.Add(consideration);

            bucket.GetUtility(aIContext);

            sub.Dispose();
            Assert.AreEqual(1, timesCalled);
        }

        [TestCase(0, 10)]
        public void GetUtility_CorrectValues_ReturnsExpected(float calculatedScore, int numberOfConsiderations)
        {
            var considerations = CreateUniformConsiderations(numberOfConsiderations, calculatedScore);

            foreach (var c in considerations)
            {
                bucket.Considerations.Add(c);
            }

            var result = bucket.GetUtility(aIContext);

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
            c1.Initialize();
            c2.Initialize();
            c3.Initialize();
            bucket.Considerations.Add(c1);
            bucket.Considerations.Add(c2);
            bucket.Considerations.Add(c3);

            var result = bucket.GetUtility(aIContext);

            Assert.AreEqual(Math.Round(expected, 2), Math.Round(result, 2));
        }

        private List<Consideration> CreateUniformConsiderations(int numberOfConsiderations, float returnValue,
            float min = 0f, float max = 1f)
        {
            var result = new List<Consideration>();
            for (var i = 0; i < numberOfConsiderations; i++)
            {
                var consideration = new Stub_Consideration_IT(returnValue, new List<Parameter>());
                consideration.Initialize();
                consideration.MinFloat.Value = min;
                consideration.MaxFloat.Value = max;
                result.Add(consideration);
            }

            return result;
        }
    }
}