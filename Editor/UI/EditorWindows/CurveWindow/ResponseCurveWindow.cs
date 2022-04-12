using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

internal class ResponseCurveWindow: EditorWindow
{
    internal ResponseCurveLcComponent ResponseCurveComponent { get; private set; }

    internal void CreateGUI()
    {
        ResponseCurveComponent = new ResponseCurveLcComponent();
        rootVisualElement.Add(ResponseCurveComponent);

    }

    internal void UpdateUi(ResponseCurve response, bool showSelection = true)
    {
        ResponseCurveComponent.UpdateUi(response, showSelection);
    }


}