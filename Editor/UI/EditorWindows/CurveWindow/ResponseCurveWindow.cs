using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

internal class ResponseCurveWindow: EditorWindow
{
    private ResponseCurveLCComponent responseCurveComponent;

    internal void CreateGUI()
    {
        responseCurveComponent = new ResponseCurveLCComponent();
        rootVisualElement.Add(responseCurveComponent);

    }

    internal void UpdateUi(ResponseCurve response, bool showSelecttion = true)
    {
        responseCurveComponent.UpdateUi(response, showSelecttion);
    }


}