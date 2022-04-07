using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

internal class ResponseCurveWindow: EditorWindow
{
    internal ResponseCurveLCComponent ResponseCurveComponent { get; private set; }

    internal void CreateGUI()
    {
        ResponseCurveComponent = new ResponseCurveLCComponent();
        rootVisualElement.Add(ResponseCurveComponent);

    }

    internal void UpdateUi(ResponseCurve response, bool showSelecttion = true)
    {
        ResponseCurveComponent.UpdateUi(response, showSelecttion);
    }


}