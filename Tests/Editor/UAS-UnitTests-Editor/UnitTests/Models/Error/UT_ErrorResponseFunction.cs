using NUnit.Framework;

namespace UnitTests.Models.Error
{
    [TestFixture]
    public class UT_ErrorResponseFunction
    {
        private Error_ResponseFunction uut;

        [SetUp]
        public void SetUp()
        {
            uut = new Error_ResponseFunction();
        }

        [Test]
        public void GetName_NoChange_ReturnsError()
        {
            var result = uut.Name;
            
            Assert.AreEqual("Error", result);
        }

        [TestCase(-10)]
        [TestCase(-0.1f)]
        [TestCase(0)]
        [TestCase(0.1f)]
        [TestCase(0.9f)]
        [TestCase(1f)]
        [TestCase(10f)]
        public void CalculateResponse_NoChange_ReturnsMinusOne(float value)
        {
            var result = uut.CalculateResponse(value,0,1);
            
            Assert.AreEqual(-1f,result);
        }
    }
}