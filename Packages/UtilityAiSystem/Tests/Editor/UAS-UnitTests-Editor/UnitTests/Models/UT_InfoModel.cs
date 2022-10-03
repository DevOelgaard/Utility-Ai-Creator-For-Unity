using NUnit.Framework;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_InfoModel
    
    {
        private InfoModel uut;

        [Test]
        public void Construct_SetInfo_ReturnsCorrectInfo()
        {
            var expected = "Some Info";
            uut = new InfoModel(expected);
            
            Assert.AreEqual(expected,uut.Info);
        }
        
        
        [Test]
        public void Construct_SetInfoType_ReturnsCorrectType()
        {
            var expected = "Some Info";
            uut = new InfoModel(expected,InfoTypes.Warning);
            
            Assert.AreEqual(InfoTypes.Warning,uut.InfoType);
        }
    }
}