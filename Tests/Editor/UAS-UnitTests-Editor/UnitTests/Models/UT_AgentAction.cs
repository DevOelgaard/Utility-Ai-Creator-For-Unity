using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using UniRx;
using System.ComponentModel;
using Mocks;

namespace UnitTests.Models
{
    [TestFixture]
    public class UT_AgentAction
    {
        private Mock_AgentAction uut;

        [SetUp]
        public void SetUp()
        {
            uut = new Mock_AgentAction();
        }

        [Test]
        public void GetTypeDescription_NoInput_ReturnsCorrectString()
        {
            var result = uut.GetTypeDescription();
            
            Assert.AreEqual("Agent Action", result);
        }

        [Test]
        public void Clone_NoInput_ReturnsCloneOfObject()
        {
            uut.Name = "UUT";
            var result = uut.Clone();
            
            Assert.AreEqual(uut.Description, result.Description);
            Assert.AreEqual(uut.Info, result.Info);
            Assert.AreEqual(uut.Name, result.Name);
            Assert.AreEqual(uut.Parameters.Count, result.Parameters.Count);
            Assert.AreEqual(uut.FileName, result.FileName);
            Assert.AreEqual(uut.HelpText, result.HelpText);
            Assert.AreNotEqual(uut.Guid,result.Guid);
        }
        [Test]
        public void Clone_NewObject_ReturnsClone()
        {
            var result = uut.Clone();
            
            Assert.AreEqual(uut.Name,result.Name);
            Assert.AreEqual(uut.Description,result.Description);
        }
    }
}
