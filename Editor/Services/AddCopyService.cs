using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

internal static class AddCopyService
{
    internal static List<string> GetChoices(Type type, List<AiObjectModel> templates = null)
    {
        var namesFromFiles = AssetDatabaseService.GetActivateAbleTypes(type);
        var tempChoices = new List<string> {Consts.LineBreakBaseTypes};
        tempChoices.AddRange(namesFromFiles
            .Where(aiObject => !aiObject.Name.Contains("Mock") && 
                           !aiObject.Name.Contains("Stub") && 
                           !aiObject.Name.Contains("Error_") &&
                           !aiObject.Name.Contains("Demo"))
            .Select(aiObject => aiObject.Name)
            .OrderBy(name => name)
            .ToList());
        
        if (templates is {Count: > 0})
        {
            tempChoices.Add(Consts.LineBreakTemplates);

            tempChoices.AddRange(templates
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToList());
        }

        if (TemplateService.Instance.IncludeDemos)
        {
            var demoChoices = namesFromFiles
                .Where(aiObject => aiObject.Name.Contains("Demo"))
                .Select(aiObject => aiObject.Name)
                .OrderBy(n => n)
                .ToList();

            if (demoChoices.Count > 0)
            {
                tempChoices.Add(Consts.LineBreakDemos);
                tempChoices.AddRange(demoChoices);
            }
        }

        return tempChoices
            .Select(StringService.SpaceBetweenUpperCase)
            .ToList();
    }

    internal static async Task<AiObjectModel> GetAiObjectClone(string name, List<AiObjectModel> templates)
    {
        var whiteSpaceName = StringService.SpaceBetweenUpperCase(name);
        var noWhiteSpace = StringService.RemoveWhiteSpaces(name);
        templates ??= new List<AiObjectModel>();
        var existingElement =
            templates.FirstOrDefault(t =>
                t.Name == name || t.Name == whiteSpaceName || t.Name == noWhiteSpace); 
        
        if(existingElement != null)
        {
            var c = await existingElement.CloneAsync();
            return c;

        } else
        {
            var c = AssetDatabaseService.GetInstanceOfType<AiObjectModel>(noWhiteSpace);
            // var c = AssetDatabaseService.GetInstanceOfType<AiObjectModel>(noWhiteSpace);
            return c;
        }
    }


}
