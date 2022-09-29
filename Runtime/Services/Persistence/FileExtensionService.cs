using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FileExtensionService
{
    internal static Type GetTypeFromFileName(string path)
    {
        if (path.Contains(Consts.FileExtension_UAI))
        {
            return typeof(Uai);
        }
        else if (path.Contains(Consts.FileExtension_Bucket))
        {
            return typeof(Bucket);
        }
        else if (path.Contains(Consts.FileExtension_Decision))
        {
            return typeof(Decision);
        }
        else if (path.Contains(Consts.FileExtension_Consideration))
        {
            return typeof(Consideration);
        }
        else if (path.Contains(Consts.FileExtension_AgentAction))
        {
            return typeof(AgentAction);
        }
        else if (path.Contains(Consts.FileExtension_ResponseCurve))
        {
            return typeof(ResponseCurve);
        }
        else if (path.Contains(Consts.FileExtension_ResponseFunction))
        {
            return typeof(ResponseFunction);
        }
        else if (path.Contains(Consts.FileExtension_Parameter))
        {
            return typeof(Parameter);
        }
        else if (path.Contains(Consts.FileExtension_TickerSettings))
        {
            return typeof(UaiTickerSettingsModel);
        }
        else if (path.Contains(Consts.FileExtension_TickerModes))
        {
            return typeof(TickerMode);
        }
        else if (path.Contains(Consts.FileExtension_UtilityContainerSelector))
        {
            return typeof(UtilityContainerSelector);
        }
        // else if (path.Contains(Consts.FileExtension_RestoreAbleCollection))
        // {
        //     return typeof(RestoreAbleCollection);
        // }
        else return null;
    }

    internal static Type GetStateTypeFromFileName(string path)
    {
        if (path.Contains(Consts.FileExtension_UAI))
        {
            return typeof(UaiSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_Bucket))
        {
            return typeof(BucketSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_Decision))
        {
            return typeof(DecisionSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_Consideration))
        {
            return typeof(ConsiderationSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_AgentAction))
        {
            return typeof(AgentActionSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_ResponseCurve))
        {
            return typeof(ResponseCurveSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_ResponseFunction))
        {
            return typeof(ResponseFunctionSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_Parameter))
        {
            return typeof(ParameterState);
        }
        else if (path.Contains(Consts.FileExtension_TickerSettings))
        {
            return typeof(UaiTickerSettingsModelSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_TickerModes))
        {
            return typeof(TickerModeSingleFileState);
        }
        else if (path.Contains(Consts.FileExtension_UtilityContainerSelector))
        {
            return typeof(UtilityContainerSelectorSingleFileState);
        }
        else return null;
    }

    internal static string GetFileExtensionFromType(Type type)
    {
        if (type.IsAssignableFrom(typeof(TemplateService)))
        {
            return Consts.FileExtension_TemplateService;
        }

        if (type.IsAssignableFrom(typeof(Uai)))
        {
            return Consts.FileExtension_UAI;
        }
        if (type.IsAssignableFrom(typeof(Bucket)))
        {
            return Consts.FileExtension_Bucket;
        }
        if (type.IsAssignableFrom(typeof(Decision)))
        {
            return Consts.FileExtension_Decision;
        }
        if (type.IsAssignableFrom(typeof(Consideration)))
        {
            return Consts.FileExtension_Consideration;
        }
        if (type.IsAssignableFrom(typeof(AgentAction)))
        {
            return Consts.FileExtension_AgentAction;
        }
        if (type.IsAssignableFrom(typeof(ResponseCurve)))
        {
            return Consts.FileExtension_ResponseCurve;
        }
        if (type.IsAssignableFrom(typeof(ResponseFunction)))
        {
            return Consts.FileExtension_ResponseFunction;
        }
        if (type.IsAssignableFrom(typeof(Parameter)))
        {
            return Consts.FileExtension_Parameter;
        }
        if (type.IsAssignableFrom(typeof(TickerMode)))
        {
            return Consts.FileExtension_TickerModes;
        }
        if (type.IsAssignableFrom(typeof(UtilityContainerSelector)))
        {
            return Consts.FileExtension_UtilityContainerSelector;
        }
        if (type.IsAssignableFrom(typeof(ProjectSettingsModel)))
        {
            return Consts.FileExtension_ProjectSettings;
        }
        if (type.IsAssignableFrom(typeof(UaiTickerSettingsModel)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        if (type.IsAssignableFrom(typeof(UasTemplateServiceSingleFileState)))
        {
            return Consts.FileExtension_TemplateService;
        }
        if (type.IsAssignableFrom(typeof(UaiSingleFileState)))
        {
            return Consts.FileExtension_UAI;
        }
        if (type.IsAssignableFrom(typeof(BucketSingleFileState)))
        {
            return Consts.FileExtension_Bucket;
        }
        if (type.IsAssignableFrom(typeof(DecisionSingleFileState)))
        {
            return Consts.FileExtension_Decision;
        }
        if (type.IsAssignableFrom(typeof(ConsiderationSingleFileState)))
        {
            return Consts.FileExtension_Consideration;
        }
        if (type.IsAssignableFrom(typeof(AgentActionSingleFileState)))
        {
            return Consts.FileExtension_AgentAction;
        }
        if (type.IsAssignableFrom(typeof(ResponseCurveSingleFileState)))
        {
            return Consts.FileExtension_ResponseCurve;
        }
        if (type.IsAssignableFrom(typeof(ResponseFunctionSingleFileState)))
        {
            return Consts.FileExtension_ResponseFunction;
        }
        if (type.IsAssignableFrom(typeof(ParameterState)))
        {
            return Consts.FileExtension_Parameter;
        }
        if (type.IsAssignableFrom(typeof(UaiTickerSettingsModelSingleFileState)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        if (type.IsAssignableFrom(typeof(TickerModeSingleFileState)))
        {
            return Consts.FileExtension_TickerModes;
        }
        if (type.IsAssignableFrom(typeof(UtilityContainerSelectorSingleFileState)))
        {
            return Consts.FileExtension_UtilityContainerSelector;
        }

        return "";
    }
}
