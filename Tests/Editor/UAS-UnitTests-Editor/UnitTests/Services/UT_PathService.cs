// using System.Threading.Tasks;
// using NUnit.Framework;
// using UnityEngine.Windows;
//
// namespace UnitTests.Services
// {
//     [TestFixture]
//     public class UT_PathService
//     {
//
//         [Test]
//         public void FormatFileName_NameShorterThanFourNoIndex_ReturnsShortenedName()
//         {
//             var restoreState = new ParameterState();
//             var name = "SomeLongFileName";
//             var expected = "Some";
//             restoreState.FileName = name;
//             restoreState.Index = -1;
//
//             var result = PathService.FormatFileName(restoreState);
//             Assert.AreEqual(expected,result);
//         }
//         
//         [Test]
//         public void FormatFileName_NameShorterThanFour_ReturnsShortenedNameWithIndex()
//         {
//             var restoreState = new ParameterState();
//             var name = "SomeLongFileName";
//             var expected = "0 Some";
//             restoreState.FileName = name;
//             restoreState.Index = 0;
//
//             var result = PathService.FormatFileName(restoreState);
//             Assert.AreEqual(expected,result);
//         }
//     }
// }