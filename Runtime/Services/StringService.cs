using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class StringService
{
    //https://www.codegrepper.com/code-examples/csharp/insert+space+between+upper+case+c%23
    internal static string SpaceBetweenUpperCase(string text)
    {
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    internal static string RemoveWhiteSpaces(string text)
    {
        var textNoWhiteSpaces = text.Replace(" ", String.Empty);
        return textNoWhiteSpaces;
    }
}
