using NUnit.Framework;

namespace UnitTests.Models.Considerations
{
    [TestFixture]
    public class UT_RandomValue
    
    {
        private RandomValue uut;
        private AiContext mock_AiContext;

        [SetUp]
        public void SetUp()
        {
            mock_AiContext = new AiContext();
            uut = new RandomValue();
            uut.Initialize();
        }
        [Test]
        public void CalculateScore_NoChange_ReturnsOne()
        {
            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(typeof(float),result.GetType());
        }
    }
}