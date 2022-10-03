using System;
using UniRx;
using UnityEditor;

internal class ResponseCurveWindow: EditorWindow
{
    internal ResponseCurveLcViewModel ResponseCurveViewModel { get; private set; }

    public IObservable<bool> OnOwnerChanged => onOwnerChanged;
    private readonly Subject<bool> onOwnerChanged = new Subject<bool>();

    internal void CreateGUI()
    {
        ResponseCurveViewModel = new ResponseCurveLcViewModel();
        rootVisualElement.Add(ResponseCurveViewModel);

    }

    internal void UpdateUi(ResponseCurve response, bool showSelection = true, string ownerName = null)
    {
        ResponseCurveViewModel.UpdateUi(response, showSelection,ownerName);
    }

    private void OnDisable()
    {
        onOwnerChanged.OnNext(true);
    }
}