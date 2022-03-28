using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

internal class ColorService
{
    internal static void SetColor(VisualElement element, float scoreNormalized)
    {
        var color = new Color(1 - scoreNormalized, scoreNormalized, 0, 0.1f);
        element.style.backgroundColor = color;
    }

    internal static void SetColor(VisualElement element, float score, float min, float max)
    {
        var normalized = (score-min)/(max-min);
        SetColor(element, normalized);
    }

    internal static void SetColor(List<KeyValuePair<VisualElement,float>> elements)
    {
        elements.Sort((a, b) => {
            if (a.Value > b.Value) return -1;
            else if (a.Value < b.Value) return 1;
            else return 0;
        });

        var stepSize = 1f;
        if (elements.Count > 1)
        {
            stepSize = 1f / ((float)elements.Count - 1);
        }
        for(var i = 0; i < elements.Count; i++)
        {
            var element = elements[i].Key;
            SetColor(element, 1-i*stepSize);
        }
    }

    internal static void ResetColor(VisualElement element)
    {
        element.style.backgroundColor = new Color(0,0,0,0);
    }
}