using System.Collections.Generic;
using Mocks;
using NUnit.Framework;

namespace UnitTests.Services
{
    [TestFixture]
    public class UT_RestoreAbleService
    {
        private List<string> nameList = new List<string>()
        {
            "One",
            "Two",
            "Three",
            "Cee",
            "Bee",
            "Dee",
            "Aaa"
        };
        
        private List<string> sortedNameList = new List<string>()
        {
            "Aaa",
            "Bee",
            "Cee",
            "Dee",
            "One",
            "Three",
            "Two",
        };
        
        [Test]
        public void NamesToList_NamedAfterNameList_ReturnsCorrectNames()
        {
            var expected = nameList;
            var aiObjects = new List<AiObjectModel>();
            foreach (var name in nameList)
            {
                var o = new Mock_Decision();
                o.Name = name;
                aiObjects.Add(o);
            }

            var result = RestoreAbleService.NamesToList(aiObjects);

            foreach (var expectedValue in expected)
            {
                var index = expected.IndexOf(expectedValue);
                Assert.AreEqual(expectedValue,result[index]);
            }
        }
        
        // [Test]
        // public void NamesToListParameters_NamedAfterNameList_ReturnsCorrectNames()
        // {
        //     var expected = nameList;
        //     var parameters = new List<ParamFloat>();
        //     foreach (var name in nameList)
        //     {
        //         var o = new ParamFloat(name,nameList.IndexOf(name));
        //         parameters.Add(o);
        //     }
        //
        //     var result = RestoreAbleService.NamesToList(parameters);
        //
        //     foreach (var expectedValue in expected)
        //     {
        //         var index = expected.IndexOf(expectedValue);
        //         Assert.AreEqual(expectedValue,result[index]);
        //     }
        // }
        
        // [Test]
        // public void OrderByNames_NamedAfterNameList_ReturnsSortedList()
        // {
        //     var expected = sortedNameList;
        //     var aiObjects = new List<AiObjectModel>();
        //     foreach (var name in nameList)
        //     {
        //         var o = new Mock_Decision();
        //         o.Name = name;
        //         aiObjects.Add(o);
        //     }
        //
        //     var result = RestoreAbleService.OrderByNames(sortedNameList,aiObjects);
        //
        //     foreach (var expectedValue in expected)
        //     {
        //         var index = expected.IndexOf(expectedValue);
        //         Assert.AreEqual(expectedValue,result[index].Name);
        //     }
        // }
    }
}