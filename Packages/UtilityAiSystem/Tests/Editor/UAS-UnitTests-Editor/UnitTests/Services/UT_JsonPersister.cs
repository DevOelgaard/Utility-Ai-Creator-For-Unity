using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.Windows;

namespace UnitTests.Services
{
    [TestFixture]
    public class UT_JsonPersister
    {
        private JsonPersister uut;
        private string testPath = Consts.PathMainFolder + "UnitTest/JsonPersister/";

        [SetUp]
        public void SetUp()
        {
            uut = new JsonPersister();
        }

        [Test]
        public void SaveObject_SimpleObject_CreatesFileAtPath()
        {
            var filePath = testPath + "SaveObject_SimpleObject_CreatesFileAtPath";
            uut.SaveObject("TestSave", filePath);

            var result = File.Exists(filePath);
            if (result)
            {
                File.Delete(filePath);
            }
            Assert.That(result);
        }


        
        [Test]
        public void SaveObjectAsync_SimpleObject_CreatesFileAtPath()
        {
            var filePath = testPath + "SaveObjectAsync_SimpleObject_CreatesFileAtPath";
            AsyncHelpers.RunSync(() => uut.SaveObjectAsync("TestSave", filePath));

            var result = File.Exists(filePath);
            if (result)
            {
                File.Delete(filePath);
            }
            Assert.That(result);
        }

        [Test]

        public void LoadObject_AfterSave_ReturnsTheSavedObject()
        {
            var filePath = testPath + "LoadObject_AfterSave_ReturnsTheSavedObject";
            uut.SaveObject("TestSave",filePath);

            var result = uut.LoadObject<string>(filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            Assert.AreEqual("TestSave",result.LoadedObject);
        }
        
        [Test]
        public void LoadObjectAsync_AfterSave_ReturnsTheSavedObject()
        {
            var filePath = testPath + "LoadObjectAsync_AfterSave_ReturnsTheSavedObject";
            uut.SaveObject("TestSave",filePath);

            var result = AsyncHelpers.RunSync(() => uut.LoadObjectAsync<string>(filePath));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            Assert.AreEqual("TestSave",result.LoadedObject);
        }
        
        [Test]
        public void LoadObjectsAsync_AfterSave_ReturnsTheSavedObjects()
        {
            var folderPath = testPath + "LoadObjectsAsync/";
            var filePath1 = folderPath + "LoadObjectsAsync_AfterSave_ReturnsTheSavedObjects1";
            var filePath2 = folderPath + "LoadObjectsAsync_AfterSave_ReturnsTheSavedObjects2";
            var filePath3 = folderPath + "LoadObjectsAsync_AfterSave_ReturnsTheSavedObjects3";
            uut.SaveObject("TestSave1",filePath1);
            uut.SaveObject("TestSave2",filePath2);
            uut.SaveObject("TestSave3",filePath3);

            var result = AsyncHelpers.RunSync(() => uut.LoadObjectsAsync<string>(folderPath));

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath);
            }
            
            Assert.AreEqual("TestSave1",result[0].LoadedObject);
            Assert.AreEqual("TestSave2",result[1].LoadedObject);
            Assert.AreEqual("TestSave3",result[2].LoadedObject);
        }
    }
}