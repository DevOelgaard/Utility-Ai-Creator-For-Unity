using NUnit.Framework;
using UniRx;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_ProjectSettingsModel
    {
        private CompositeDisposable disposable = new CompositeDisposable();
        private ProjectSettingsModel uut;
        
        [SetUp]
        public void SetUp()
        {
            uut = new ProjectSettingsModel();
        }

        [Test]
        public void CurrentProjectPath_ChangePath_OnCurrenProjectPathChangedInvoked()
        {
            var result = "";
            uut.OnCurrentProjectPathChanged
                .Subscribe(newName => result = newName)
                .AddTo(disposable);

            uut.CurrentProjectPath = "TestPath";
            
            Assert.AreEqual("TestPath",result);
        }
        
        [Test]
        public void CurrentProjectName_ChangeName_CurrentProjectNameIsNewName()
        {
            uut.CurrentProjectName = "TestPath";
            
            Assert.AreEqual("TestPath",uut.CurrentProjectName);
        }

        [TearDown]
        public void TearDown()
        {
            disposable.Clear();
        }
    }
}