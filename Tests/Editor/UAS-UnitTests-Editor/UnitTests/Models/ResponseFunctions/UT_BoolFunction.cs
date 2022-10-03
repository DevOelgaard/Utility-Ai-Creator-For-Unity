using System;
using NUnit.Framework;

namespace UnitTests.Models.ResponseFunctions
{
    [TestFixture]
    public class UT_BoolFunction
    {
        private BoolFunction uut;

        [SetUp]
        public void SetUp()
        {
            uut = new BoolFunction();
            uut.ParameterContainer.GetParamFloat("CutOff").Value = 0.5f;
        }

        [TestCase(-1f)]
        [TestCase(0f)]
        [TestCase(0.49f)]
        public void CalculateResponse_BelowCutOff_ReturnsMax(float value)
        {
            var expected = Convert.ToSingle(uut.Max.Value);
            
            var result = uut.CalculateResponse(value, 0, 1);
            
            Assert.AreEqual(expected,result);
        }
        
        [TestCase(0.5f)]
        [TestCase(0.51f)]
        [TestCase(0.7f)]
        [TestCase(0.99f)]
        [TestCase(1f)]
        [TestCase(100f)]
        public void CalculateResponse_AboveCutOff_ReturnsMin(float value)
        {
            var expected = Convert.ToSingle(uut.Min.Value);
            
            var result = uut.CalculateResponse(value, 0, 1);
            
            Assert.AreEqual(expected,result);
        }
    }
}