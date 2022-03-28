using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;
using System.Linq;

[TestFixture]
public class UT_ScorerService
{
    ScorerService uut;

    [SetUp]
    public void SetUp()
    {
        uut = ScorerService.Instance;
    }

    //[TestCase("UCSHighestScore")]
    //public void LoadContainerSelectors_Constructor_CorrectClassLoaded(string className)
    //{
    //    var result = uut.ContainerSelectors.Values
    //        .FirstOrDefault(e=>e.GetType().ToString() == className);
        
    //    Assert.That(result != null);
    //}


    //[Test]
    //public void LoadContainerSelectors_Constructor_NoDuplicates()
    //{
    //    var result = uut.ContainerSelectors.Values
    //        .GroupBy(uS => uS.GetType())
    //        .Where(group => group.Count() > 1)
    //        .ToList()
    //        .Count();

    //    Assert.That(result == 0);
    //}

    [Test]
    public void LoadUtilityScorers_Constructor_MoreThanZeroLoaded()
    {
        var result = uut.UtilityScorers.Values.Count;
        Assert.That(result > 0);
    }

    [TestCase("USAverageScorer")]
    [TestCase("USCompensationScorer")]
    public void LoadUtilityScorers_Constructor_CorrectClassLoaded(string className)
    {
        var result = uut.UtilityScorers.Values
            .FirstOrDefault(e => e.GetType().ToString() == className);

        Assert.That(result != null);
    }
}
