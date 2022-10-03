using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.IO;
using Mocks;
using UnityEngine.Assertions.Must;

namespace IntegrationTests.Persistence
{
    [TestFixture]
    public class IT_04JsonPersisterPersistenceApiDecision
    {
        private PersistenceAPI sut;
        private string testPath = Consts.PathMainFolder + "IT/Persitence/04";
        private List<string> directoriesToDelete = new List<string>();
        private readonly Type testType = typeof(Decision);

        [SetUp]
        public void SetUp()
        {
            sut = PersistenceAPI.Instance;
            Directory.CreateDirectory(testPath);
        }
        
        [Test]
        public void SaveObjectAsync_OneFile_NewFileSaved()
        {
            var filePath = testPath + "SaveObjectAsync_OneFile_NewFileSaved";
            var saveFile = new Decision()
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + FileExtensionService.GetFileExtensionFromType(testType);

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));

            var result = File.Exists(expectedPath);

            DeleteFolder(filePath);
            Assert.That(result);
        }
        
        [Test]
        public void LoadObjectPath_CorrectPath_ReturnsObjectState()
        {
            var filePath = testPath + "LoadObjectPath_CorrectPath_ReturnsObjectState";
            var saveFile = new Decision()
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + FileExtensionService.GetFileExtensionFromType(testType);

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));

            var result = sut.LoadObjectPath<DecisionSingleFileState>(expectedPath);

            DeleteFolder(filePath);
            Assert.AreEqual("T1",result.LoadedObject.Name);
        }
        
        private void DeleteFolder(string path)
        {
            Directory.Delete(path,true);
        }

        ~IT_04JsonPersisterPersistenceApiDecision()
        {
            DeleteFolder(testPath);
        }
    }
}