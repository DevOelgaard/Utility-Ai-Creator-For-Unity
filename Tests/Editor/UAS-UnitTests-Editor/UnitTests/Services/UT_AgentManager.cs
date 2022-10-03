using System.Collections.Generic;
using Mocks;
using NUnit.Framework;
using UniRx;

namespace UnitTests.Services
{
    [TestFixture]
    public class UT_AgentManager
    {
        private CompositeDisposable disposable = new CompositeDisposable();
        private AgentManager uut;

        [SetUp]
        public void SetUp()
        {
            uut = AgentManager.Instance;
            uut.Reset();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void RegisterAgent_RegisterNumberOfAgents_NumberOfAgentsRegistered(int numberOfAgents)
        {
            for (int i = 0; i < numberOfAgents; i++)
            {
                uut.Register(new Mock_Agent());
            }

            var result = uut.Model.Agents.Count;
            
            Assert.AreEqual(numberOfAgents,result);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void RegisterAgent_RegisterNumberOfAgents_InvokesAgentsUpdatedNumberOfTimes(int numberOfAgents)
        {
            var result = 0;
            uut.AgentsUpdated
                .Subscribe(_ => result++)
                .AddTo(disposable);
            
            for (int i = 0; i < numberOfAgents; i++)
            {
                uut.Register(new Mock_Agent());
            }
            
            Assert.AreEqual(numberOfAgents,result);
        }
        
        [TestCase(0,0,0)]
        [TestCase(1,1,2)]
        [TestCase(2,1,2)]
        [TestCase(0,3,1)]
        [TestCase(4,0,1)]
        public void RegisterAgent_RegisterNumberOfAgents_InvokesAgentTypesUpdatedNumberOfTimes(int numberOfTypeOneAgents, int numberOfTypeTwoAgents, int expected)
        {
            var result = 0;
            uut.AgentIdentifiersUpdated
                .Subscribe(_ => result++)
                .AddTo(disposable);
            
            var agents = new List<IAgent>();
            for (int i = 0; i < numberOfTypeOneAgents; i++)
            {
                var agent = new Mock_Agent
                {
                    TypeIdentifierString = "TypeOne"
                };
                agents.Add(agent);
                uut.Register(agent);
            }
            
            for (int i = 0; i < numberOfTypeTwoAgents; i++)
            {
                var agent = new Mock_Agent
                {
                    TypeIdentifierString = "TypeTwo"
                };
                agents.Add(agent);
                uut.Register(agent);
            }
            Assert.AreEqual(expected,result);
        }
        
        [TestCase(0,0)]
        [TestCase(1,1)]
        [TestCase(2,1)]
        [TestCase(3,3)]
        [TestCase(4,2)]
        public void UnRegisterAgent_RegisterNumberOfAgents_NumberOfAgentsRegistered(int numberOfAgents, int numberToRemove)
        {
            var expected = numberOfAgents - numberToRemove;
            var agents = new List<IAgent>();
            for (int i = 0; i < numberOfAgents; i++)
            {
                var agent = new Mock_Agent();
                agents.Add(agent);
                uut.Register(agent);
            }

            for (int i = 0; i < numberToRemove; i++)
            {
                uut.Unregister(agents[i]);
            }
            var result = uut.Model.Agents.Count;
            
            Assert.AreEqual(expected,result);
        }

        [TestCase(0,0)]
        [TestCase(1,1)]
        [TestCase(2,1)]
        [TestCase(3,3)]
        [TestCase(4,2)]
        public void UnRegisterAgent_RegisterNumberOfAgents_InvokesAgentsUpdatedNumberOfTimes(int numberOfAgents, int numberToRemove)
        {
            var agents = new List<IAgent>();
            for (int i = 0; i < numberOfAgents; i++)
            {
                var agent = new Mock_Agent();
                agents.Add(agent);
                uut.Register(agent);
            }

            var result = 0;
            uut.AgentsUpdated
                .Subscribe(_ => result++)
                .AddTo(disposable);
            
            for (int i = 0; i < numberToRemove; i++)
            {
                uut.Unregister(agents[i]);
            }
            
            Assert.AreEqual(numberToRemove,result);
        }
        
        [TestCase(0,0)]
        [TestCase(1,1)]
        [TestCase(2,1)]
        [TestCase(3,3)]
        [TestCase(4,2)]
        public void GetAgentsByIdentifier_AfterAddingXAgentsOfTypeOneAndYAgentsOfTypeTwo_CorrectNumberOfAgents(int numberOfTypeOneAgents, int numberOfTypeTwoAgents)
        {
            var agents = new List<IAgent>();
            var expected = numberOfTypeOneAgents;
            for (int i = 0; i < numberOfTypeOneAgents; i++)
            {
                var agent = new Mock_Agent
                {
                    TypeIdentifierString = "TypeOne"
                };
                agents.Add(agent);
                uut.Register(agent);
            }
            
            for (int i = 0; i < numberOfTypeTwoAgents; i++)
            {
                var agent = new Mock_Agent
                {
                    TypeIdentifierString = "TypeTwo"
                };
                agents.Add(agent);
                uut.Register(agent);
            }

            var result = uut.GetAgentsByIdentifier("TypeOne").Count;

            Assert.AreEqual(expected,result);
        }
        

        [TearDown]
        public void TearDown()
        {
            disposable.Clear();
        }
    }
}