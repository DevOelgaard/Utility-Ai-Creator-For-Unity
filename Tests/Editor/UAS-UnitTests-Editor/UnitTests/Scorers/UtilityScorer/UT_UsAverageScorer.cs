using System.Collections;
using System.Collections.Generic;
using Mocks;
using NUnit.Framework;
using NSubstitute;


namespace UnitTests.Scorers.UtilityScorers
{
        [TestFixture]
        public class UT_UsAverageScorer
        {
                private IUtilityScorer uut;
                private IAiContext mockAiContext;

                private List<Consideration> mockConsiderations;

                [SetUp]
                public void SetUp()
                {
                        uut = new UsAverageScorer();
                        mockAiContext = Substitute.For<IAiContext>();
                        mockConsiderations = new List<Consideration>()
                        {
                                new Mock_ConsiderationSimple(),
                                new Mock_ConsiderationSimple(),
                                new Mock_ConsiderationSimple(),
                                new Mock_ConsiderationSimple(),
                                new Mock_ConsiderationSimple(),
                        };
                }

                [Test]
                public void CalculateUtility_NoConsiderations_ReturnsZero()
                {
                        var considerationsZeroElements = new List<Consideration>();

                        var result = uut.CalculateUtility(considerationsZeroElements, mockAiContext);

                        Assert.AreEqual(0, result);
                }

                [Test]
                public void CalculateUtility_AllConsiderationsAreModifiers_ReturnOne()
                {

                        foreach (var consideration in mockConsiderations)
                        {
                                var cast = consideration as Mock_ConsiderationSimple;
                                cast.SetIsModifier(true);
                        }

                        var result = uut.CalculateUtility(mockConsiderations, mockAiContext);
                        Assert.AreEqual(1, result);
                }

                [Test]
                public void CalculateUtility_AllConsiderationsScoreOne_ReturnOne()
                {

                        foreach (var consideration in mockConsiderations)
                        {
                                var cast = consideration as Mock_ConsiderationSimple;
                                cast.SetIsModifier(false);
                                cast.ReturnValue = 1;
                        }

                        var result = uut.CalculateUtility(mockConsiderations, mockAiContext);
                        Assert.AreEqual(1, result);
                }
        }
}