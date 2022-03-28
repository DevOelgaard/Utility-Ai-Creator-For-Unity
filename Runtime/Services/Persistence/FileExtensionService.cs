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

    internal static string GetFileExtension(Type type, object o)
    {
        var start = "";
        //var end = "";

        if (type.IsAssignableFrom(typeof(RestoreAbleCollection)))
        {
            var cast = o as RestoreAbleCollection;
            type = cast.Type;
            //end = Consts.FileExtension_RestoreAbleCollection;
        }

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

        if (type.IsAssignableFrom(typeof(UASTemplateService)))
        {
            start = Consts.FileExtension_UasTemplateCollection;
        }

        if (type.IsAssignableFrom(typeof(AiTickerSettingsModel)))
        {
            start = Consts.FileExtension_TickerSettings;
        }

        return start;// +end;
    }

}
