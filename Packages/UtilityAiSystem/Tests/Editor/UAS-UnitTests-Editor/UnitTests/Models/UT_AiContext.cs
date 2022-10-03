using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using Mocks;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_AiContext
    {
        private IAiContext uut;

        [SetUp]
        public void SetUp()
        {
            uut = new AiContext();
        }

        [Test]
        public void GetContext_AfterSetting_ReturnsContext()
        {
            var key = "TestKey";
            float value = 1f;
            uut.SetContext(key, value);
            var result = uut.GetContext<float>(key);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void SetAgent_ToANewAgent_AgentIsNewAgent()
        {
            var newAgent = new Mock_Agent();
            
            uut.Agent = newAgent;
            
            Assert.AreEqual(newAgent,uut.Agent);
        }

        [Test]
        public void GetContext_OverWritingInitialValueWithNewValueOfDifferentType_ReturnsSecondValue()
        {
            var key = "Testkey";
            int original = 2;
            string newValue = "A new Value";

            uut.SetContext(key, original);
            uut.SetContext(key, newValue);

            var result = uut.GetContext<string>(key);

            Assert.AreEqual(newValue, result);
        }

        [Test]
        public void RemoveContext_RemovesValueAfterSettingGetContext_ReturnsNull()
        {
            var key = "TestKey";
            int original = 2;

            uut.SetContext(key, original);
            uut.RemoveContext(key);
            var result = uut.GetContext<string>(key);

            Assert.That(result == null);
        }
    }
}