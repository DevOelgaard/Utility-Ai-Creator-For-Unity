using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.IO;
using Mocks;

namespace IntegrationTests.Persistence
{
    [TestFixture]
    public class IT_01JsonPersisterPersistenceApiAgentAction
    {
        private PersistenceAPI sut;
        private string testPath = Consts.PathMainFolder + "IT/Persitence/01";

        private readonly Type testType = typeof(AgentAction);

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
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));

            var result = File.Exists(expectedPath);

            DeleteFolder(filePath);
            Assert.That(result);
        }
        
        [Test]
        public void SaveObjectAsync_TwoFiles_BothFilesRemains()
        {
            var filePath = testPath + "SaveObjectAsync_TwoFiles_BothFilesRemains";
            var saveFile1 = new Mock_AgentAction
            {
                Name = "T1"
            };
            var saveFile2 = new Mock_AgentAction
            {
                Name = "T2"
            };
            var expectedPath1 = filePath + "/" + saveFile1.Name + "." + Consts.FileExtension_AgentAction;
            var expectedPath2 = filePath + "/" + saveFile2.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile1, filePath));
            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile2, filePath));

            var result1 = File.Exists(expectedPath1);
            var result2 = File.Exists(expectedPath2);
            Assert.That(result1);
            Assert.That(result2);
            DeleteFolder(filePath);
        }
        
        [Test]
        public void SaveObjectDestructivelyAsync_NoPreExistingFiles_FileSaved()
        {
            var filePath = testPath + "SaveObjectDestructivelyAsync_NoPreExistingFiles_FileSaved";
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectDestructivelyAsync(saveFile, filePath));

            var result = File.Exists(expectedPath);
            Assert.That(result);
            DeleteFolder(filePath);
        }
        
        [Test]
        public void SaveObjectDestructivelyAsync_OnePreExistingFiles_NewFileSavedOldFilesDeleted()
        {
            var filePath = testPath + "SaveObjectDestructivelyAsync_OnePreExistingFiles_NewFileSavedOldFilesDeleted";
            var preExistingFile = new Mock_AgentAction()
            {
                Name = "Pre"
            };
            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(preExistingFile, filePath));
            var expectedDeleted = filePath +"/" + preExistingFile.Name + "." + Consts.FileExtension_AgentAction;

            
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectDestructivelyAsync(saveFile, filePath));

            var fileSaved = File.Exists(expectedPath);
            Assert.That(fileSaved);
            DeleteFolder(filePath);
        }
        
        [Test]
        public void SaveObjectDestructively_VariousFolders_RemovesFolder()
        {
            var filePath = testPath + "SaveObjectDestructively_VariousFolders_RemovesFolder";
            File.WriteAllText("Full", filePath+"/full");
            Directory.CreateDirectory(filePath + "/empty");
            
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectDestructivelyAsync(saveFile, filePath));

            var fileSaved = File.Exists(expectedPath);
            var fullDirectoryDeleted = !Directory.Exists(filePath+"/full");
            var emptyDirectoryDeleted = !Directory.Exists(filePath + "/empty");
            
            Assert.That(fileSaved);
            Assert.That(fullDirectoryDeleted);
            Assert.That(emptyDirectoryDeleted);
            
            DeleteFolder(filePath);
        }

        [Test]
        public void LoadObjectPath_CorrectPath_ReturnsObject()
        {
            var filePath = testPath + "LoadObjectPath_CorrectPath_ReturnsObject";
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));

            var result = sut.LoadObjectPath<AgentActionSingleFileState>(expectedPath);

            DeleteFolder(filePath);
            Assert.AreEqual("T1",result.LoadedObject.Name);
        }
        
        [Test]
        public void LoadObjectPathAsync_CorrectPath_ReturnsObject()
        {
            var filePath = testPath + "LoadObjectPathAsync_CorrectPath_ReturnsObject";
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var expectedPath = filePath +"/" + saveFile.Name + "." + Consts.FileExtension_AgentAction;

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));

            var result = AsyncHelpers.RunSync(() => sut.LoadObjectPathAsync<AgentActionSingleFileState>(expectedPath));

            DeleteFolder(filePath);
            Assert.AreEqual("T1",result.LoadedObject.Name);
        }
        
        [Test]
        public void LoadObjectsOfTypeAsync_AgentAction_ReturnsObjectsMatchingPattern()
        {
            var filePath = testPath + "LoadObjectsOfTypeAsync_AgentAction_ReturnsObjectsMatchingPattern";
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var saveFile2 = new Mock_ConsiderationSimple()
            {
                Name = "W1"
            };

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));
            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile2, filePath));

            var result = AsyncHelpers.RunSync(() => sut.LoadObjectsOfTypeAsync<AgentActionSingleFileState>(filePath,typeof(AgentAction)));

            DeleteFolder(filePath);
            Assert.AreEqual("T1",result[0].LoadedObject.Name);
            Assert.AreEqual(1,result.Count);
        }        
        
        [Test]
        public void LoadObjectsAsync_SimpleSearchPattern_ReturnsObjectsMatchingPattern()
        {
            var filePath = testPath + "LoadObjectsOfTypeAsync_AgentAction_ReturnsObjectsMatchingPattern";
            var saveFile = new Mock_AgentAction
            {
                Name = "T1"
            };
            var saveFile2 = new Mock_ConsiderationSimple()
            {
                Name = "W1"
            };

            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile, filePath));
            AsyncHelpers.RunSync(() => sut.SaveObjectAsync(saveFile2, filePath));

            var result = AsyncHelpers.RunSync(() => sut.LoadObjectsAsync<AgentActionSingleFileState>(filePath,Consts.FileExtension_AgentAction));

            DeleteFolder(filePath);
            Assert.AreEqual("T1",result[0].LoadedObject.Name);
            Assert.AreEqual(1,result.Count);
        }

        private void DeleteFolder(string path)
        {
            Directory.Delete(path,true);
        }

        ~IT_01JsonPersisterPersistenceApiAgentAction()
        {
            DeleteFolder(testPath);
        }
    }
}