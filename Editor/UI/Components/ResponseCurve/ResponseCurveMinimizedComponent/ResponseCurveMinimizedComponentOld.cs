// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UniRx;
// using UnityEngine.UIElements;
//
// public class ResponseCurveMinimizedComponentOld : VisualElement
// {
//     private ResponseCurve responseCurve;
//     private string ownerName;
//     
//     private readonly Label nameLabel;
//     private readonly DropdownField curveDropDown;
//     private readonly LineChartButton responseCurveButton;
//     private ResponseCurveWindow responseCurveWindow;
//     private readonly Toggle inverseToggle;
//
//     public IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
//     private readonly Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();
//
//     private readonly CompositeDisposable rcDisposable = new CompositeDisposable();
//     private readonly CompositeDisposable disposable = new CompositeDisposable();
//
//     public ResponseCurveMinimizedComponentOld()
//     {
//         var root = AssetDatabaseService.GetTemplateContainer(GetType());
//         Add(root);
//         styleSheets.Add(StylesService.GetStyleSheet("ResponseCurveMinimizedComponent"));
//
//         // Header
//         nameLabel = root.Q<Label>("Name-Label");
//         curveDropDown = root.Q<DropdownField>("ResponseCurve-DropDown");
//         TemplateService.Instance.ResponseCurves.OnValueChanged
//             .Subscribe(InitCurveDropDownChoices)
//             .AddTo(disposable);
//         InitCurveDropDownChoices(TemplateService.Instance.ResponseCurves.Values);
//         curveDropDown.SetValueWithoutNotify(null);
//         curveDropDown.RegisterCallback<ChangeEvent<string>>(OnCurveDropdownValueChanged);
//         
//         // Body
//         responseCurveButton = new LineChartButton
//         {
//             name = "ResponseCurveButton"
//         };
//         responseCurveButton.RegisterCallback<MouseUpEvent>(_ =>
//         {
//             responseCurveWindow = WindowOpener.GetNewResponseCurveWindow();
//             if (responseCurve == null) return;
//             responseCurveWindow.UpdateUi(responseCurve);
//
//             // rcDisposable.Clear();
//             // responseCurveWindow.ResponseCurveComponent.OnResponseCurveChanged
//             //     .Subscribe(UpdateUiNoNotifyWindow)
//             //     .AddTo(rcDisposable);
//         });
//
//         var body = root.Q<VisualElement>("Body");
//         body.Add(responseCurveButton);
//         
//         // Footer
//         var saveTemplateButton = root.Q<Button>("SaveTemplateButton");
//         saveTemplateButton.RegisterCallback<MouseUpEvent>(SaveTemplate);
//
//         inverseToggle = root.Q<Toggle>("IsInversed-Toggle");
//         inverseToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
//         {
//             responseCurve.IsInversed = evt.newValue;
//         });
//     }
//
//     private void InitCurveDropDownChoices(List<AiObjectModel> responseCurves)
//     {
//         curveDropDown.choices = AddCopyService.GetChoices(typeof(ResponseCurve),responseCurves);
//     }
//
//     public void UpdateUi(ResponseCurve rC, string newOwnerName = null)
//     {
//         SetResponseCurve(rC);
//         nameLabel.text = responseCurve.Name;
//         if (!string.IsNullOrEmpty(newOwnerName))
//         {
//             nameLabel.text += " - " + newOwnerName;
//         }
//         responseCurveButton.UpdateUi(responseCurve);
//     }
//
//     private void UpdateUiNoNotifyWindow(ResponseCurve rC)
//     {
//         SetResponseCurve(rC,false);
//         responseCurve = rC;
//         nameLabel.text = responseCurve.Name;
//         if (!string.IsNullOrEmpty(ownerName))
//         {
//             nameLabel.text += " - " + ownerName;
//         }
//         responseCurveButton.UpdateUi(responseCurve);
//     }
//     
//     private async void OnCurveDropdownValueChanged(ChangeEvent<string> evt)
//     {
//         if (evt.newValue is null or null) return;
//         var template = TemplateService
//             .Instance
//             .ResponseCurves
//             .Values
//             .FirstOrDefault(e => e.Name == evt.newValue);
//         
//         if (template != null)
//         {
//             var clone = await template.CloneAsync() as ResponseCurve;
//             SetResponseCurve(clone);
//         } else
//         {
//             SetResponseCurve(AssetDatabaseService.GetInstanceOfType<ResponseCurve>(evt.newValue));
//         }
//         curveDropDown.SetValueWithoutNotify(null);
//     }
//
//     private void SetResponseCurve(ResponseCurve newResponseCurve, bool updateWindow = true, bool windowNotify = false)
//     {
//         responseCurve = newResponseCurve;
//         if (updateWindow && responseCurveWindow != null)
//         {
//             responseCurveWindow.UpdateUi(responseCurve, true, windowNotify);
//         }
//         rcDisposable.Clear();
//         responseCurve.OnCurveValueChanged
//             .Subscribe(_ =>
//             {
//                 UpdateUiNoNotifyWindow(responseCurve);
//             })
//             .AddTo(rcDisposable);
//         
//         onResponseCurveChanged.OnNext(responseCurve);
//     }
//
//     private async void SaveTemplate(MouseUpEvent evt)
//     {
//         var clone = await responseCurve.CloneAsync();
//         TemplateService.Instance.Add(clone);
//     }
//
//     ~ResponseCurveMinimizedComponentOld()
//     {
//         rcDisposable.Clear();
//         disposable.Clear();
//     }
// }
