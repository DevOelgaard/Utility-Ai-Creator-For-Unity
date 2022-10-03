using NUnit.Framework;

namespace UnitTests.Models.Considerations
{
    [TestFixture]
    public class UT_FixedValue
    
    {
        private FixedValue uut;
        private AiContext mock_AiContext;

        [SetUp]
        public void SetUp()
        {
            mock_AiContext = new AiContext();
            uut = new FixedValue();
            uut.Initialize();
        }
        [Test]
        public void CalculateScore_NoChange_ReturnsOne()
        {
            var result = uut.CalculateScore(mock_AiContext);
            
            Assert.AreEqual(1,result);
        }
    }
}