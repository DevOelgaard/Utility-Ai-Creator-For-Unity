using Mocks;
using NUnit.Framework;

namespace UnitTests.Models.Considerations
{
    [TestFixture]
    public class UT_DoNotRepeat
    {
        private DoNotRepeat uut;
        private AiContext mock_AiContext;

        [SetUp]
        public void SetUp()
        {
            mock_AiContext = new AiContext();
            uut = new DoNotRepeat();
            uut.Initialize();
        }

        [Test]
        public void CalculateScore_FirstTimeRunTypeNotToRepeatDecision_ReturnsOne()
        {
            var parent = new Mock_Decision();
            parent.Initialize();
            mock_AiContext.LastSelectedBucket = new Mock_Bucket();
            mock_AiContext.CurrentEvaluatedBucket = new Mock_Bucket();
            mock_AiContext.LastSelectedDecision = new Mock_Decision();
            mock_AiContext.CurrentEvaluatedDecision = parent;
            parent.Considerations.Add(uut);
            parent.ForceUpdateInfo();

            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(1,result);
        }
        
        [Test]
        public void CalculateScore_MultipleTimesRunDoTypeNotToRepeatDecision_ReturnsZero()
        {
            var parent = new Mock_Decision();
            parent.Initialize();
            mock_AiContext.LastSelectedBucket = new Mock_Bucket();
            mock_AiContext.CurrentEvaluatedBucket = mock_AiContext.LastSelectedBucket;
            mock_AiContext.LastSelectedDecision = parent;
            mock_AiContext.CurrentEvaluatedDecision = parent;
            parent.Considerations.Add(uut);
            parent.ForceUpdateInfo();

            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(0,result);
        }
        
        [Test]
        public void CalculateScore_FirstTimeRunTypeNotToRepeatBucket_ReturnsOne()
        {
            var parent = new Mock_Bucket();
            parent.Initialize();
            mock_AiContext.LastSelectedBucket = new Mock_Bucket();
            mock_AiContext.CurrentEvaluatedBucket = parent;
            mock_AiContext.LastSelectedDecision = new Mock_Decision();
            mock_AiContext.CurrentEvaluatedDecision = new Mock_Decision();
            parent.Considerations.Add(uut);
            parent.ForceUpdateInfo();

            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(1,result);
        }
        
        [Test]
        public void CalculateScore_MultipleTimesRunDoTypeNotToRepeatBucket_ReturnsZero()
        {
            var parent = new Mock_Bucket();
            parent.Initialize();
            mock_AiContext.LastSelectedBucket = parent;
            mock_AiContext.CurrentEvaluatedBucket = parent;
            mock_AiContext.LastSelectedDecision = new Mock_Decision();
            mock_AiContext.CurrentEvaluatedDecision = new Mock_Decision();
            parent.Considerations.Add(uut);
            parent.ForceUpdateInfo();

            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(0,result);
        }
        
        [Test]
        public void CalculateScore_NoParent_ReturnsOne()
        {
            mock_AiContext.LastSelectedBucket = new Mock_Bucket();
            mock_AiContext.CurrentEvaluatedBucket = new Mock_Bucket();
            mock_AiContext.LastSelectedDecision = new Mock_Decision();
            mock_AiContext.CurrentEvaluatedDecision = new Mock_Decision();

            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(1,result);
        }
    }
}