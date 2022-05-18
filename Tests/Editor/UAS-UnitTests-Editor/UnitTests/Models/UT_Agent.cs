using NUnit.Framework;
using Mocks;
using UniRx;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_Agent
    {
        private CompositeDisposable disposable = new CompositeDisposable();
        private Mock_Agent uut;

        [SetUp]
        public void SetUp()
        {
            uut = new Mock_Agent();
        }

        [Test]
        public void SetName_ChangingNameOfAgent_InvokesOnNameChanged()
        {
            var result = 0;
            uut.Model.OnNameChanged
                .Subscribe(_ => result++)
                .AddTo(disposable);

            uut.Model.Name = "New Name";
            
            Assert.AreEqual(1,result);
        }
        
        [Test]
        public void SetName_ChangingNameOfAgent_AgentsNameIsNewName()
        {
            uut.Model.Name = "New Name";

            var result = uut.Model.Name;
            
            Assert.AreEqual("New Name",result);
        }


        [TearDown]
        public void TearDown()
        {
            disposable.Clear();
        }
    }
}