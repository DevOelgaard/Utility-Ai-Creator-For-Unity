using NUnit.Framework;

namespace UnitTests.Models.Error
{
    [TestFixture]
    public class UT_ErrorConsideration
    {
        private Error_Consideration uut;
        [SetUp]
        public void SetUp()
        {
            uut = new Error_Consideration();
        }

        [Test]
        public void CalculateBaseScore_NoChange_ReturnsMinusOne()
        {
            var result = uut.CalculateScore(default);
            
            Assert.AreEqual(0,result);
        }
        
        [Test]
        public void GetTypeDescription_NoChange_ReturnsErrorConsideration()
        {
            var result = uut.GetTypeDescription();
            
            Assert.AreEqual("Error Consideration",result);
        }
    }
}