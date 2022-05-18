using NUnit.Framework;
using UniRx;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_ParameterEnum
    {
        private CompositeDisposable disposable = new CompositeDisposable();
        private ParameterEnum uut;

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Clone_NewlyCreatedObject_CloneHasSameValueAndName()
        {
            uut = new ParameterEnum("TestOne", PerformanceTag.High);

            var result = uut.Clone() as ParameterEnum;
            
            Assert.AreEqual(uut.Name,result.Name);
            Assert.AreEqual(uut.Value,result.Value);
            Assert.AreEqual(uut.EnumType,result.EnumType);
            Assert.AreEqual(uut.ParameterEnum,result.ParameterEnum);
        }
        
        [TearDown]
        public void TearDown()
        {
            disposable.Clear();
        }
    }
}