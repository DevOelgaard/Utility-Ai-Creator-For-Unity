using NUnit.Framework;
using Tests.Editor.UnitTests;

namespace UnitTests.Models.ResponseFunctions
{
    [TestFixture]
    public class UT_InverseLogistic
    {
        private InverseLogisticFunction uut;
        
        [SetUp]
        public void SetUp()
        {
            uut = new InverseLogisticFunction();
        }
        
        [TestCase(0.1f,0.3415f)]
        [TestCase(0.3f,0.4389f)]
        [TestCase(0.5f,0.5f)]
        [TestCase(0.9f,0.6585f)]
        public void CalculateResponse_NoChange_ReturnsExpected(float x,float expected)
        {
            var result = uut.CalculateResponse(x, 0, 1);
            
            Assert.That(TestHelpers.FloatEqualTwoDecimals(result,expected));
        }
    }
}