using System;
using UniRx;
using UnityEditor;

internal class ResponseCurveWindow: EditorWindow
{
    internal ResponseCurveLcComponent ResponseCurveComponent { get; private set; }

    public IObservable<bool> OnOwnerChanged => onOwnerChanged;
    private readonly Subject<bool> onOwnerChanged = new Subject<bool>();

    internal void CreateGUI()
    {
        ResponseCurveComponent = new ResponseCurveLcComponent();
        rootVisualElement.Add(ResponseCurveComponent);

    }

    internal void UpdateUi(ResponseCurve response, bool showSelection = true, string ownerName = null)
    {
        ResponseCurveComponent.UpdateUi(response, showSelection,ownerName);
    }

    private void OnDisable()
    {
        onOwnerChanged.OnNext(true);
    }
}