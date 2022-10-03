using NUnit.Framework;
using Mocks;
using UniRx;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_Parameter
    {
        private CompositeDisposable disposable = new CompositeDisposable();
        private Parameter uut;

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Clone_NewlyCreatedObject_CloneHasSameValueAndName()
        {
            uut = new Parameter("TestOne", 5);

            var result = uut.Clone();
            
            Assert.AreEqual(uut.Name,result.Name);
            Assert.AreEqual(uut.Value,result.Value);
        }
        
        [TearDown]
        public void TearDown()
        {
            disposable.Clear();
        }
    }
}