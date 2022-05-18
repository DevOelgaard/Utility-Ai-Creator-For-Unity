using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using Mocks;

namespace UnitTests.Scorers.UtilityContainerSelectors
{
    [TestFixture]
    public class UT_UCSHighestScore
    {
        private UCSHighestScore uut;
        private List<Decision> mock_Decisions;
        private List<Bucket> mock_Buckets;
        private AiContext mockAiContext;

        [SetUp]
        public void SetUp()
        {
            uut = new UCSHighestScore();

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

            foreach (var mockDecision in mock_Decisions)
            {
                mockDecision.Initialize();
            }

            foreach (var mockBucket in mock_Buckets)
            {
                mockBucket.Initialize();
            }

            #endregion

            mockAiContext = new AiContext();
            mockAiContext.TickMetaData = new TickMetaData();
        }

        [Test]
        public void Constructor_Default_ReturnsObject()
        {
            Assert.That(uut != null);
        }

        [Test]
        public void GetName_Default_ReturnsCorrectName()
        {
            Assert.AreEqual(Consts.UCS_HighestScore_Name, uut.GetName());
        }

        [Test]
        public void GetDescription_Default_ReturnsCorrectDescription()
        {
            Assert.AreEqual(Consts.UCS_HighestScore_Description, uut.GetDescription());
        }


        [TestCase("decisions")]
        [TestCase("buckets")]
        public void GetBestUtilityContainer_OneScoreHigher_ReturnsHighestScore(string collection)
        {
            if (collection == "decisions")
            {
                var highest = new Mock_Decision(float.MaxValue);
                highest.Name = "Highest";
                mock_Decisions.Add(highest);

                // To make sure the return isn't the last object in collection
                var anotherLow = new Mock_Decision(6.2f);
                anotherLow.Name = "anotherLow";
                mock_Decisions.Add(anotherLow);

                var result = uut.GetBestUtilityContainer(mock_Decisions, mockAiContext);

                Assert.AreEqual("Highest", result.Name);
                return;
            }
            else if (collection == "buckets")
            {
                var highest = new Mock_Bucket(float.MaxValue);
                highest.Initialize();
                highest.Name = "Highest";
                var highestDecision = new Mock_Decision(float.MaxValue);
                highestDecision.Initialize();
                highestDecision.Name = "HighestDecision";
                highest.Decisions.Add(highestDecision);
                mock_Buckets.Add(highest);
                

                // To make sure the return isn't the last object in collection
                var anotherLow = new Mock_Bucket(6.2f);
                anotherLow.Name = "anotherLow";
                mock_Buckets.Add(anotherLow);

                var result = uut.GetBestUtilityContainer(mock_Buckets, mockAiContext);

                Assert.AreEqual("Highest", result.Name);
                return;
            }

            // Catch all failure
            Assert.That(false);
        }

        [TestCase("decisions")]
        [TestCase("buckets")]
        public void GetBestUtilityContainer_TwoHighestScores_ReturnsFirst(string collection)
        {
            if (collection == "decisions")
            {
                var highest = new Mock_Decision(10.1f);
                highest.Name = "Highest";

                var highest2 = new Mock_Decision(10.1f);
                highest2.Name = "Highest2";

                mock_Decisions.Add(highest);
                mock_Decisions.Add(highest2);

                var result = uut.GetBestUtilityContainer(mock_Decisions, mockAiContext);
                Assert.AreEqual("Highest", result.Name);
                return;
            }
            else if (collection == "buckets")
            {
                var highest = new Mock_Bucket(10.1f);
                highest.Name = "Highest";

                var highest2 = new Mock_Bucket(10.1f);
                highest2.Name = "Highest2";

                mock_Buckets.Add(highest);
                mock_Buckets.Add(highest2);

                var result = uut.GetBestUtilityContainer(mock_Buckets, mockAiContext);
                Assert.AreEqual("Highest", result.Name);
                return;
            }

            // Catch all failure
            Assert.That(false);
        }

        [TestCase("decisions")]
        [TestCase("buckets")]
        public void GetBestUtilityContainer_NoElementsInCollection_ReturnsNull(string collection)
        {
            if (collection == "decisions")
            {
                mock_Decisions = new List<Decision>();
                var result = uut.GetBestUtilityContainer(mock_Decisions, mockAiContext);
                Assert.AreEqual(null, result);
                return;
            }
            else if (collection == "buckets")
            {
                mock_Buckets = new List<Bucket>();
                var result = uut.GetBestUtilityContainer(mock_Buckets, mockAiContext);
                Assert.AreEqual(null, result);
                return;
            }

            // Catch all failure
            Assert.That(false);
        }

        [TestCase("decisions")]
        [TestCase("buckets")]
        public void GetBestUtilityContainer_AllElementsNegative_ReturnsNull(string collection)
        {
            if (collection == "decisions")
            {
                mock_Decisions = new List<Decision>();
                for (var i = 0; i < 15; i++)
                {
                    var d = new Mock_Decision(-i);
                    mock_Decisions.Add(d);
                }

                var result = uut.GetBestUtilityContainer(mock_Decisions, mockAiContext);
                Assert.AreEqual(null, result);
                return;
            }
            else if (collection == "buckets")
            {
                mock_Buckets = new List<Bucket>();
                for (var i = 0; i < 15; i++)
                {
                    var b = new Mock_Bucket(-i);
                    mock_Buckets.Add(b);
                }

                var result = uut.GetBestUtilityContainer(mock_Buckets, mockAiContext);
                Assert.AreEqual(null, result);
                return;
            }

            // Catch all failure
            Assert.That(false);
        }
    }
}