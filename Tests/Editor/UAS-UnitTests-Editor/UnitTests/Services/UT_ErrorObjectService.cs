using NUnit.Framework;

namespace UnitTests.Services
{
    [TestFixture]
    public class UT_ErrorObjectService
    {
        [Test]
        public void GetErrorObject_Consideration_ReturnsErrorConsideration()
        {
            var result = ErrorObjectService.GetErrorObject(typeof(Consideration));
            
            Assert.AreEqual(typeof(Error_Consideration),result.GetType());
        }
        
        [Test]
        public void GetErrorObject_AgentAction_ReturnsErrorAgentAction()
        {
            var result = ErrorObjectService.GetErrorObject(typeof(AgentAction));
            
            Assert.AreEqual(typeof(Error_Action),result.GetType());
        }
        
        [Test]
        public void GetErrorObject_AiObjectModel_ReturnsErrorAiObjectModel()
        {
            var result = ErrorObjectService.GetErrorObject(typeof(Decision));
            
            Assert.AreEqual(typeof(Decision),result.GetType());
        }
    }
}