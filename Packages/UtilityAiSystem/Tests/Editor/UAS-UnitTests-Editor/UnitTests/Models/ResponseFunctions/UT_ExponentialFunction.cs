using NUnit.Framework;

namespace UnitTests.Models.ResponseFunctions
{
    [TestFixture]
    public class UT_ExponentialFunction
    {
        private ExponentialFunction uut;
        
        [SetUp]
        public void SetUp()
        {
            uut = new ExponentialFunction();
        }

        [TestCase(0,1,0)]
        [TestCase(1,1,0.01f)]
        [TestCase(1,2,0.01f)]
        [TestCase(2,2,0.04f)]
        [TestCase(2,10,10.24f)]
        [TestCase(-1,2,0.01f)]
        [TestCase(-2,2,0.04f)]
        [TestCase(2,-2,0.0025f)]
        public void CalculateResponse_NoChange_ReturnsExpected(float x, float power, float expected)
        {
            uut.ParameterContainer.GetParamFloat("Power").Value = power;
            var result = uut.CalculateResponse(x, 0, 100);
            
            Assert.AreEqual(expected,result);
        }
    }
}