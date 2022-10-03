using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using System.Linq;

namespace UnitTests.Scorers
{
    [TestFixture]
    public class UT_ScorerService
    {
        ScorerService uut;

        [SetUp]
        public void SetUp()
        {
            uut = ScorerService.Instance;
        }

        [Test]
        public void LoadUtilityScorers_Constructor_MoreThanZeroLoaded()
        {
            var result = uut.UtilityScorers.Values.Count;
            Assert.That(result > 0);
        }

        [TestCase("UsAverageScorer")]
        [TestCase("USCompensationScorer")]
        public void LoadUtilityScorers_Constructor_CorrectClassLoaded(string className)
        {
            var result = uut.UtilityScorers.Values
                .FirstOrDefault(e => e.GetType().ToString() == className);

            Assert.That(result != null);
        }
    }
}