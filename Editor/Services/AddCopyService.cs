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
        var tempChoices = namesFromFiles
            .Where(t => !t.Name.Contains("Mock") && !t.Name.Contains("Stub") && !t.Name.Contains("Error_"))
            .Select(t => t.Name)
            .OrderBy(t => t)
            .ToList();

        if (!UasTemplateService.Instance.IncludeDemos)
        {
            tempChoices = tempChoices.Where(t => !t.Contains("Demo")).ToList();
        }

        var choices = new List<string>();
        foreach(var c in tempChoices)
        {
            choices.Add(StringService.SpaceBetweenUpperCase(c));
        }

        return choices;
    }


}
