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
using UnityEditor.Rendering;

namespace UnitTests.Models
{
        [TestFixture]
        public class UT_ResponseCurve
        {
                private CompositeDisposable disposable = new CompositeDisposable();
                private ResponseCurve uut;

                [SetUp]
                public void SetUp()
                {
                        uut = new ResponseCurve();
                }

                [TestCase(1)]
                [TestCase(2)]
                [TestCase(3)]
                [TestCase(4)]
                [TestCase(5)]
                [TestCase(6)]
                [TestCase(7)]
                public void AddResponseFunction_AddResponseFunctions_ExpectedNumberOfSegments(int numberOfFunctions)
                {
                        var expectedNumberOfSegments = numberOfFunctions - 1;

                        for (int i = 0; i < numberOfFunctions; i++)
                        {
                                uut.AddResponseFunction(new Mock_ResponseFunction());
                        }

                        var result = uut.Segments.Count;

                        Assert.AreEqual(expectedNumberOfSegments, result);
                }

                [TestCase(0)]
                [TestCase(1)]
                [TestCase(2)]
                [TestCase(3)]
                [TestCase(4)]
                [TestCase(5)]
                [TestCase(6)]
                [TestCase(7)]
                public void AddResponseFunction_InvokeOnParameterChangedOnNewFunction_InvokesOnCurveValueChanged(
                        int timesToInvoke)
                {
                        var function = new Mock_ResponseFunction();
                        uut.AddResponseFunction(function);
                        var timesInvoked = 0;
                        uut.OnCurveValueChanged
                                .Subscribe(_ => timesInvoked++)
                                .AddTo(disposable);

                        for (var i = 0; i < timesToInvoke; i++)
                        {
                                function.InvokeParametersChanged();
                        }

                        Assert.AreEqual(timesToInvoke, timesInvoked);
                }

                [Test]
                public void UpdateFunction_UpdatingWithCorrectFunction_FunctionsUpdatedCorrectly()
                {
                        var first = new Mock_ResponseFunction();
                        var second = new Mock_ResponseFunction();
                        var third = new Mock_ResponseFunction();
                        uut.AddResponseFunction(first);
                        uut.AddResponseFunction(second);
                        var expected = uut.ResponseFunctions.IndexOf(second);

                        uut.UpdateFunction(second, third);
                        var result = uut.ResponseFunctions.IndexOf(third);

                        Assert.AreEqual(expected, result);
                }

                [Test]
                public void RemoveFunction_RemoveTwoFunctions_ZeroFunctionsRemaining()
                {
                        var expected = 0;
                        var first = new Mock_ResponseFunction();
                        var second = new Mock_ResponseFunction();
                        uut.AddResponseFunction(first);
                        uut.AddResponseFunction(second);

                        uut.RemoveResponseFunction(second);
                        uut.RemoveResponseFunction(first);

                        var result = uut.ResponseFunctions.Count;
                        Assert.AreEqual(expected, result);
                }

                [TearDown]
                public void TearDown()
                {
                        uut = new ResponseCurve();
                        disposable.Clear();
                }
        }
}