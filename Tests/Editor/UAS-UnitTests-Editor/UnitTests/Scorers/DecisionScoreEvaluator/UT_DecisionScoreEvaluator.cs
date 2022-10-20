using System.Collections.Generic;
using NUnit.Framework;
using Mocks;

namespace UnitTests.Scorers.DecisionScoreEvaluators
{
    [TestFixture]
    public class UtDecisionScoreEvaluator
    {
        private DseHighestBuckets uut;
        private List<Decision> mockDecisions;
        private List<Bucket> mockBuckets;
        private Mock_UtilityContainerSelector mockDecisionSelector;
        private Mock_UtilityContainerSelector mockBucketSelector;
        private Mock_Uai mockUai;

        [SetUp]
        public void SetUp()
        {
            #region Init Mock Decisions

            var d1 = new Mock_Decision();
            var d2 = new Mock_Decision(5);
            var d3 = new Mock_Decision(-5);
            var d4 = new Mock_Decision(9.9f);
            var d5 = new Mock_Decision(10.0f);
            var d6 = new Mock_Decision(9.8f);

            mockDecisions = new List<Decision>
            {
                d1,
                d2,
                d3,
                d4,
                d5,
                d6
            };
            foreach (var decision in mockDecisions)
            {
                decision.Initialize();
            }

            #endregion

            #region Init Mock Buckets

            var b1 = new Mock_Bucket();
            var b2 = new Mock_Bucket(5);
            var b3 = new Mock_Bucket(-5);
            var b4 = new Mock_Bucket(9.9f);
            var b5 = new Mock_Bucket(10.0f);
            var b6 = new Mock_Bucket(9.8f);

            mockBuckets = new List<Bucket>
            {
                b1,
                b2,
                b3,
                b4,
                b5,
                b6
            };

            foreach (var bucket in mockBuckets)
            {
                bucket.Initialize();
            }
            for (var i = 0; i < mockBuckets.Count; i++)
            {
                mockBuckets[i].Decisions.Add(mockDecisions[i]);
            }

            #endregion

            mockDecisionSelector = new Mock_UtilityContainerSelector();
            mockBucketSelector = new Mock_UtilityContainerSelector();

            mockUai = new Mock_Uai
            {
                CurrentBucketSelector = mockBucketSelector,
                CurrentDecisionSelector = mockDecisionSelector
            };

            uut = new DseHighestBuckets();
        }

        [Test]
        public void GetName_Default_ReturnsCorrectName()
        {
            Assert.AreEqual(Consts.Name_HighestBucketDSE, uut.GetName());
        }

        [Test]
        public void GetDescription_Default_ReturnsCorrectDescription()
        {
            Assert.AreEqual(Consts.Description_HighestBucketDSE, uut.GetDescription());
        }

        [Test]
        public void NextActions_Decisions_ReturnsCorrectCollection()
        {
            var highest = new Mock_Decision(11);
            mockDecisions.Add(highest);

            var someLow = new Mock_Decision(2.5f);
            mockDecisions.Add(someLow);

            mockDecisionSelector.BestDecision = highest;


            var result = uut.NextActions(mockDecisions, mockUai);

            Assert.AreEqual(highest.AgentActions.Values, result);
        }

        [Test]
        public void NextActions_DecisionsNoElements_ReturnsZeroActions()
        {
            mockDecisions = new List<Decision>();

            var result = uut.NextActions(mockDecisions, mockUai);

            Assert.AreEqual(0, result.Count);
        }

        [TestCase(0)]
        [TestCase(-0.0000001f)]
        [TestCase(-1)]
        [TestCase(-9999999)]
        public void NextActions_BestDecisionScoreZeroOrLower_ReturnsZeroActions(float returnValue)
        {
            var failDecision = new Mock_Decision(returnValue);
            mockDecisionSelector.BestDecision = failDecision;

            var result = uut.NextActions(mockDecisions, mockUai);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void NextActions_BucketsNoElements_ReturnsZeroActions()
        {
            mockBuckets = new List<Bucket>();
           
            var result = uut.NextActions(mockBuckets, mockUai);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void NextActions_BucketsOneElement_ReturnsCorrectCollection()
        {
            var highest = new Mock_Bucket(15.5f);
            highest.Initialize();
            var highDecision = new Mock_Decision(222f);
            highDecision.Initialize();
            var resultAction = new Mock_AgentAction()
            {
                Name = "Result"
            };
            resultAction.Initialize();
            highDecision.AgentActions.Add(resultAction);
            highest.Decisions.Add(highDecision);
            foreach (var d in mockDecisions)
            {
                highest.Decisions.Add(d);
            }
            mockBuckets = new List<Bucket>
            {
                highest
            };

            var result = uut.NextActions(mockBuckets, mockUai);

            Assert.AreEqual(highDecision.AgentActions.Values, result);
        }
        
        [Test]
        public void NextActions_BucketsCorrectValues_ReturnsCorrectCollection()
        {
            var highestBucket = new Mock_Bucket(15.5f);
            highestBucket.Initialize();
            var highDecision = new Mock_Decision(222f);
            highDecision.Initialize();
            highestBucket.Decisions.Add(highDecision);
            foreach (var d in mockDecisions)
            {
                highestBucket.Decisions.Add(d);
            }

            var resultAction = new Mock_AgentAction()
            {
                Name = "Result"
            };
            resultAction.Initialize();
            highDecision.AgentActions.Add(resultAction);
        
            mockBuckets.Add(highestBucket);
 
            var result = uut.NextActions(mockBuckets, mockUai);

            Assert.AreEqual(highDecision.AgentActions.Values, result);
        }

        [Test]
        public void NextActions_HighestScoredBucketHasNoValidDecisions_ReturnsActionsFromSecondBucket()
        {
            var highValue = 15.5f;
            var lowValue = 0.1f;
            var invalidValue = 0f;

            mockBuckets = new List<Bucket>();
            var highBucket = new Mock_Bucket(highValue);
            highBucket.Initialize();
            var invalidDecision = new Mock_Decision(invalidValue);
            invalidDecision.Initialize();
            highBucket.Decisions.Add(invalidDecision);
            mockBuckets.Add(highBucket);

            var lowBucket = new Mock_Bucket(lowValue);
            lowBucket.Initialize();
            var validDecision = new Mock_Decision(highValue);
            validDecision.Initialize();
            lowBucket.Decisions.Add(validDecision);
            mockBuckets.Add(lowBucket);
            
            var resultAction = new Mock_AgentAction()
            {
                Name = "Result"
            };
            resultAction.Initialize();
            validDecision.AgentActions.Add(resultAction);

            var result = uut.NextActions(mockBuckets, mockUai);

            Assert.AreEqual(validDecision.AgentActions.Values, result);
        }
    }
}