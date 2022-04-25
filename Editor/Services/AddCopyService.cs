using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

internal static class AddCopyService
{
    internal static List<string> GetChoices(Type t)
    {
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(t);
        var tempChoices = new List<string>();
        tempChoices.Add(Consts.LineBreakBaseTypes);
        tempChoices.AddRange(namesFromFiles
            .Where(type => !type.Name.Contains("Mock") && 
                           !type.Name.Contains("Stub") && 
                           !type.Name.Contains("Error_") &&
                           !type.Name.Contains("Demo"))
            .Select(type => type.Name)
            .OrderBy(name => name)
            .ToList());

        if (TemplateService.Instance.IncludeDemos)
        {
            tempChoices.Add(Consts.LineBreakDemos);
            var demoChoices = namesFromFiles
                .Where(type => type.Name.Contains("Demo"))
                .Select(type => type.Name)
                .OrderBy(n => n)
                .ToList();

            tempChoices.AddRange(demoChoices);
        }

        return tempChoices
            .Select(StringService.SpaceBetweenUpperCase)
            .ToList();
    }


}
