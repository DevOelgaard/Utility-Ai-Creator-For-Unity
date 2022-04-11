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
            .Where(type => !type.Name.Contains("Mock") && !type.Name.Contains("Stub") && !type.Name.Contains("Error_"))
            .Select(type => type.Name)
            .OrderBy(type => type)
            .ToList();

        if (!UasTemplateService.Instance.IncludeDemos)
        {
            tempChoices = tempChoices.Where(type => !type.Contains("Demo")).ToList();
        }

        return tempChoices
            .Select(StringService.SpaceBetweenUpperCase)
            .ToList();
    }


}
