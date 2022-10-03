using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.TestTools;
using UnityEngine;
using System;

namespace UnitTests.Services
{
        [TestFixture]
        public class UT_FileExtensionService
        {
                [TestCase(Consts.FileExtension_UAI, typeof(Uai))]
                [TestCase(Consts.FileExtension_Bucket, typeof(Bucket))]
                [TestCase(Consts.FileExtension_Decision, typeof(Decision))]
                [TestCase(Consts.FileExtension_Consideration, typeof(Consideration))]
                [TestCase(Consts.FileExtension_AgentAction, typeof(AgentAction))]
                [TestCase(Consts.FileExtension_ResponseCurve, typeof(ResponseCurve))]
                [TestCase(Consts.FileExtension_ResponseFunction, typeof(ResponseFunction))]
                [TestCase(Consts.FileExtension_Parameter, typeof(Parameter))]
                [TestCase(Consts.FileExtension_TickerSettings, typeof(UaiTickerSettingsModel))]
                [TestCase(Consts.FileExtension_TickerModes, typeof(TickerMode))]
                [TestCase(Consts.FileExtension_UtilityContainerSelector, typeof(UtilityContainerSelector))]
                public void GetTypeFromFileName_CorrectExtension_ReturnsExpectedType(string fileName, Type expectedType)
                {
                        var result = FileExtensionService.GetTypeFromFileName(fileName);
                        
                        Assert.AreEqual(expectedType,result);
                }
                
                [TestCase(Consts.FileExtension_UAI, typeof(UaiSingleFileState))]
                [TestCase(Consts.FileExtension_Bucket, typeof(BucketSingleFileState))]
                [TestCase(Consts.FileExtension_Decision, typeof(DecisionSingleFileState))]
                [TestCase(Consts.FileExtension_Consideration, typeof(ConsiderationSingleFileState))]
                [TestCase(Consts.FileExtension_AgentAction, typeof(AgentActionSingleFileState))]
                [TestCase(Consts.FileExtension_ResponseCurve, typeof(ResponseCurveSingleFileState))]
                [TestCase(Consts.FileExtension_ResponseFunction, typeof(ResponseFunctionSingleFileState))]
                [TestCase(Consts.FileExtension_Parameter, typeof(ParameterState))]
                [TestCase(Consts.FileExtension_TickerSettings, typeof(UaiTickerSettingsModelSingleFileState))]
                [TestCase(Consts.FileExtension_TickerModes, typeof(TickerModeSingleFileState))]
                [TestCase(Consts.FileExtension_UtilityContainerSelector, typeof(UtilityContainerSelectorSingleFileState))]
                public void GetStateFromFileName_CorrectExtension_ReturnsExpectedType(string fileName, Type expectedType)
                {
                        var result = FileExtensionService.GetStateTypeFromFileName(fileName);
                        
                        Assert.AreEqual(expectedType,result);
                }
                
                [TestCase(Consts.FileExtension_UAI, typeof(Uai))]
                [TestCase(Consts.FileExtension_Bucket, typeof(Bucket))]
                [TestCase(Consts.FileExtension_Decision, typeof(Decision))]
                [TestCase(Consts.FileExtension_Consideration, typeof(Consideration))]
                [TestCase(Consts.FileExtension_AgentAction, typeof(AgentAction))]
                [TestCase(Consts.FileExtension_ResponseCurve, typeof(ResponseCurve))]
                [TestCase(Consts.FileExtension_ResponseFunction, typeof(ResponseFunction))]
                [TestCase(Consts.FileExtension_Parameter, typeof(Parameter))]
                [TestCase(Consts.FileExtension_TickerSettings, typeof(UaiTickerSettingsModel))]
                [TestCase(Consts.FileExtension_TickerModes, typeof(TickerMode))]
                [TestCase(Consts.FileExtension_UtilityContainerSelector, typeof(UtilityContainerSelector))]
                [TestCase(Consts.FileExtension_ProjectSettings, typeof(ProjectSettingsModel))]
                [TestCase(Consts.FileExtension_UAI, typeof(UaiSingleFileState))]
                [TestCase(Consts.FileExtension_Bucket, typeof(BucketSingleFileState))]
                [TestCase(Consts.FileExtension_Decision, typeof(DecisionSingleFileState))]
                [TestCase(Consts.FileExtension_Consideration, typeof(ConsiderationSingleFileState))]
                [TestCase(Consts.FileExtension_AgentAction, typeof(AgentActionSingleFileState))]
                [TestCase(Consts.FileExtension_ResponseCurve, typeof(ResponseCurveSingleFileState))]
                [TestCase(Consts.FileExtension_ResponseFunction, typeof(ResponseFunctionSingleFileState))]
                [TestCase(Consts.FileExtension_Parameter, typeof(ParameterState))]
                [TestCase(Consts.FileExtension_TickerSettings, typeof(UaiTickerSettingsModelSingleFileState))]
                [TestCase(Consts.FileExtension_TickerModes, typeof(TickerModeSingleFileState))]
                [TestCase(Consts.FileExtension_UtilityContainerSelector, typeof(UtilityContainerSelectorSingleFileState))]
                [TestCase(Consts.FileExtension_TemplateService, typeof(TemplateService))]
                [TestCase(Consts.FileExtension_TemplateService, typeof(UasTemplateServiceSingleFileState))]
                [TestCase("", typeof(float))]
                public void GetFileExtensionFromType_CorrectType_ReturnsExpectedExtension(string expectedExtension, Type type)
                {
                        var result = FileExtensionService.GetFileExtensionFromType(type);
                        
                        Assert.AreEqual(expectedExtension,result);
                }
        }
}
