using Mocks;
using NUnit.Framework;

namespace IntegrationTests.Restoring
{
    [TestFixture]
    public class IT_RestoreAbleRestoreStateAllUaiObjects
    {
        private string testPath = Consts.PathMainFolder + "IT/Rest/01/";

        [Test]
        public void RestoreFromState_AgentAction_ObjectIsRestored()
        {
            var type = typeof(AgentAction);
            var sut = new Mock_AgentAction
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            var folderPath = testPath + sut.Name;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<AgentActionState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<AgentAction>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
        
        [Test]
        public void RestoreFromState_Consideration_ObjectIsRestored()
        {
            var type = typeof(Consideration);
            var sut = new Mock_ConsiderationSimple()
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            
            var folderPath = testPath + type;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<ConsiderationState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<Consideration>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
        
        [Test]
        public void RestoreFromState_Decision_ObjectIsRestored()
        {
            var type = typeof(Decision);
            var sut = new Decision()
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            
            var folderPath = testPath + type;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<DecisionState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<Decision>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
        
        [Test]
        public void RestoreFromState_Bucket_ObjectIsRestored()
        {
            var type = typeof(Bucket);
            var sut = new Bucket()
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            
            var folderPath = testPath + type;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<BucketState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<Bucket>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
        
        [Test]
        public void RestoreFromState_Uai_ObjectIsRestored()
        {
            var type = typeof(Uai);
            var sut = new Uai()
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            
            var folderPath = testPath + type;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<UaiState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<Uai>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
        
        [Test]
        public void RestoreFromState_ResponseCurve_ObjectIsRestored()
        {
            var type = typeof(ResponseCurve);
            var sut = new ResponseCurve()
            {
                Name = "N1",
                Description = "D1"
            };
            sut.Initialize();
            
            var folderPath = testPath + type;
            var filePath = folderPath+"/"+sut.Name+ "."+FileExtensionService.GetFileExtensionFromType(type);
            AsyncHelpers.RunSync(() => PersistenceAPI.Instance.SaveObjectAsync(sut, folderPath));

            var state = PersistenceAPI.Instance.LoadObjectPath<ResponseCurveState>(filePath);
            var result = AsyncHelpers.RunSync(() => RestoreAble.Restore<ResponseCurve>(state.LoadedObject));
            
            Assert.AreEqual(sut.Name,result.Name);
            Assert.AreEqual(sut.Description,result.Description);
        }
    }
}