using NUnit.Framework;

namespace UnitTests.Models.ResponseFunctions
{
    [TestFixture]
    public class UT_FixedValueFunction
    {
        private FixedValueFunction uut;
        
        [SetUp]
        public void SetUp()
        {
            uut = new FixedValueFunction();
        }
        
        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void CalculateResponse_NoChange_ReturnsExpected(float x)
        {
            var result = uut.CalculateResponse(x, 0, 100);
            
            Assert.AreEqual(1,result);
        }
    }
}