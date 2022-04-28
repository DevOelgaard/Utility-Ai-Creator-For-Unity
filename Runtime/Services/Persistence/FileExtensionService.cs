using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FileExtensionService
{
    internal static string GetExtension(object o)
    {
        var type = o.GetType();
        return GetFileExtension(type, o);
    }

    internal static Type GetTypeFromFileName(string path)
    {
        if (path.Contains(Consts.FileExtension_UAI))
        {
            return typeof(Ai);
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
            return typeof(AiTickerSettingsModel);
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

    internal static Type GetStateFromFileName(string path)
    {
        if (path.Contains(Consts.FileExtension_UAI))
        {
            return typeof(AiState);
        }
        else if (path.Contains(Consts.FileExtension_Bucket))
        {
            return typeof(BucketState);
        }
        else if (path.Contains(Consts.FileExtension_Decision))
        {
            return typeof(DecisionState);
        }
        else if (path.Contains(Consts.FileExtension_Consideration))
        {
            return typeof(ConsiderationState);
        }
        else if (path.Contains(Consts.FileExtension_AgentAction))
        {
            return typeof(AgentActionState);
        }
        else if (path.Contains(Consts.FileExtension_ResponseCurve))
        {
            return typeof(ResponseCurveState);
        }
        else if (path.Contains(Consts.FileExtension_ResponseFunction))
        {
            return typeof(ResponseFunctionState);
        }
        else if (path.Contains(Consts.FileExtension_Parameter))
        {
            return typeof(ParameterState);
        }
        else if (path.Contains(Consts.FileExtension_TickerSettings))
        {
            return typeof(AiTickerSettingsState);
        }
        else if (path.Contains(Consts.FileExtension_TickerModes))
        {
            return typeof(TickerModeState);
        }
        else if (path.Contains(Consts.FileExtension_UtilityContainerSelector))
        {
            return typeof(UCSState);
        }
        // else if (path.Contains(Consts.FileExtension_RestoreAbleCollection))
        // {
        //     return typeof(RestoreAbleCollectionState);
        // }
        else return null;
    }


    internal static string GetFileExtension(Type type, object o)
    {
        var start = "";
        //var end = "";

        // if (type.IsAssignableFrom(typeof(RestoreAbleCollection)))
        // {
        //     var cast = o as RestoreAbleCollection;
        //     type = cast.Type;
        //     //end = Consts.FileExtension_RestoreAbleCollection;
        // }

        if (type.IsAssignableFrom(typeof(AgentAction)))
        {
            start = Consts.FileExtension_AgentAction;
        }

        if (type.IsAssignableFrom(typeof(Consideration)))
        {
            start = Consts.FileExtension_Consideration;
        }

        if (type.IsAssignableFrom(typeof(Decision)))
        {
            start = Consts.FileExtension_Decision;
        }

        if (type.IsAssignableFrom(typeof(Bucket)))
        {
            start = Consts.FileExtension_Bucket;
        }

        if (type.IsAssignableFrom(typeof(Ai)))
        {
            start = Consts.FileExtension_UAI;
        }

        if (type.IsAssignableFrom(typeof(TemplateService)))
        {
            start = Consts.FileExtension_TemplateService;
        }

        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            start = Consts.FileExtension_TickerSettings;
        }

        return start;// +end;
    }

    internal static string GetFileExtensionFromType(Type type)
    {
        if (type.IsAssignableFrom(typeof(TemplateService)))
        {
            return Consts.FileExtension_TemplateService;
        }

        if (type.IsAssignableFrom(typeof(Ai)))
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
        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        if (type.IsAssignableFrom(typeof(TickerMode)))
        {
            return Consts.FileExtension_TickerModes;
        }
        if (type.IsAssignableFrom(typeof(UtilityContainerSelector)))
        {
            return Consts.FileExtension_UtilityContainerSelector;
        }
        // if (type.IsAssignableFrom(typeof(RestoreAbleCollection)))
        // {
        //     return Consts.FileExtension_RestoreAbleCollection;
        // }
        // if (type.IsAssignableFrom(typeof(AiTickerState)))
        // {
        //     return Consts.FileExtension_AiTicker;
        // }
        if (type.IsAssignableFrom(typeof(ProjectSettingsModel)))
        {
            return Consts.FileExtension_TemplateService;
        }
        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            return Consts.FileExtension_TickerSettings;
        }

        if (type.IsAssignableFrom(typeof(UasTemplateServiceState)))
        {
            return Consts.FileExtension_TemplateService;
        }

        if (type.IsAssignableFrom(typeof(AiState)))
        {
            return Consts.FileExtension_UAI;
        }
        if (type.IsAssignableFrom(typeof(BucketState)))
        {
            return Consts.FileExtension_Bucket;
        }
        if (type.IsAssignableFrom(typeof(DecisionState)))
        {
            return Consts.FileExtension_Decision;
        }
        if (type.IsAssignableFrom(typeof(ConsiderationState)))
        {
            return Consts.FileExtension_Consideration;
        }
        if (type.IsAssignableFrom(typeof(AgentActionState)))
        {
            return Consts.FileExtension_AgentAction;
        }
        if (type.IsAssignableFrom(typeof(ResponseCurveState)))
        {
            return Consts.FileExtension_ResponseCurve;
        }
        if (type.IsAssignableFrom(typeof(ResponseFunctionState)))
        {
            return Consts.FileExtension_ResponseFunction;
        }
        if (type.IsAssignableFrom(typeof(ParameterState)))
        {
            return Consts.FileExtension_Parameter;
        }
        if (type.IsAssignableFrom(typeof(AiTickerSettingsState)))
        {
            return Consts.FileExtension_TickerSettings;
        }
        if (type.IsAssignableFrom(typeof(TickerModeState)))
        {
            return Consts.FileExtension_TickerModes;
        }
        if (type.IsAssignableFrom(typeof(UCSState)))
        {
            return Consts.FileExtension_UtilityContainerSelector;
        }
        // if (type.IsAssignableFrom(typeof(RestoreAbleCollectionState)))
        // {
        //     return Consts.FileExtension_RestoreAbleCollection;
        // }
        // if (type.IsAssignableFrom(typeof(AiTickerState)))
        // {
        //     return Consts.FileExtension_AiTicker;
        // }
        if (type.IsAssignableFrom(typeof(ProjectSettingsModel)))
        {
            return Consts.FileExtension_TemplateService;
        }
        if (type.IsAssignableFrom(typeof(AiTickerSettingsState)))
        {
            return Consts.FileExtension_TickerSettings;
        }

        return "";
    }
}
