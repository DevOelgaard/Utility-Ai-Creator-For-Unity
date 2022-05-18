using System;
using System.Collections.Generic;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
    public class ResponseCurveMinimizedViewModel : VisualElement
    {
        private ResponseCurve responseCurve;
        private string ownerName;

        private readonly DropdownField curveDropDown;
        private readonly LineChartButton responseCurveButton;
        private readonly ToolbarToggle inverseToggle;
        private ResponseCurveWindow responseCurveWindow;
        
        public IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
        private readonly Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();

        private readonly CompositeDisposable rcDisposable = new CompositeDisposable();
        private readonly CompositeDisposable rcWindowDisposable = new CompositeDisposable();
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public ResponseCurveMinimizedViewModel()
        {
            var root = AssetService.GetTemplateContainer(GetType());
            Add(root);
            styleSheets.Add(StylesService.GetStyleSheet("ResponseCurveMinimizedComponent"));

            // Header
            curveDropDown = root.Q<DropdownField>("ResponseCurve-DropDown");
            TemplateService.Instance.ResponseCurves.OnValueChanged
                .Subscribe(InitCurveDropDownChoices)
                .AddTo(disposable);
            InitCurveDropDownChoices(TemplateService.Instance.ResponseCurves.Values);
            curveDropDown.SetValueWithoutNotify(null);
            curveDropDown.RegisterCallback<ChangeEvent<string>>(OnCurveDropdownValueChanged);
        
            // Body
            responseCurveButton = new LineChartButton
            {
                name = "ResponseCurveButton"
            };

            UpdateResponseCurveWindow();
            responseCurveButton.RegisterCallback<MouseUpEvent>(_ =>
            {
                if (responseCurveWindow == null)
                {
                    responseCurveWindow = WindowOpener.GetNewResponseCurveWindow();
                }
                UpdateResponseCurveWindow();
            });

            var body = root.Q<VisualElement>("Body");
            body.Add(responseCurveButton);
        
            // Footer
            var saveTemplateButton = root.Q<Button>("SaveTemplateButton");
            saveTemplateButton.RegisterCallback<MouseUpEvent>(SaveTemplate);

            inverseToggle = root.Q<ToolbarToggle>("Inverse-ToolbarToggle");
            inverseToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                responseCurve.IsInversed = evt.newValue;
            });
        }

        public void UpdateUi(ResponseCurve newResponseCurve, string newOwnerName)
        {
            responseCurve = newResponseCurve;
            ownerName = newOwnerName;
            rcDisposable.Clear();
            responseCurve.OnCurveValueChanged
                .Subscribe(ReDrawCurve)
                .AddTo(rcDisposable);
            responseCurve.OnFunctionsChanged
                .Subscribe(ReDrawCurve)
                .AddTo(rcDisposable);
            inverseToggle.SetValueWithoutNotify(responseCurve.IsInversed);
            curveDropDown.SetValueWithoutNotify(responseCurve.Name);

            if (responseCurveWindow != null)
            {
                responseCurveWindow.UpdateUi(responseCurve);
            }
            responseCurveButton.UpdateUi(responseCurve);
        }

        private void ReDrawCurve(bool notUsed)
        {
            responseCurveButton.UpdateUi(responseCurve);
        }

        private async void SaveTemplate(MouseUpEvent evt)
        {
            var clone = await responseCurve.CloneAsync();
            TemplateService.Instance.Add(clone);
            curveDropDown.SetValueWithoutNotify(responseCurve.Name);
        }

        private async void OnCurveDropdownValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue != null && evt.newValue != Consts.LineBreakBaseTypes &&
                evt.newValue != Consts.LineBreakTemplates && evt.newValue != Consts.LineBreakDemos)
            {
                var rC =
                    await AddCopyService.GetAiObjectClone(evt.newValue, TemplateService.Instance.ResponseCurves.Values);
                
                onResponseCurveChanged.OnNext(rC as ResponseCurve);
            }
            // curveDropDown.SetValueWithoutNotify(responseCurve.Name);
        }
        
        private void InitCurveDropDownChoices(List<AiObjectModel> responseCurveTemplates)
        {
            curveDropDown.choices = AddCopyService.GetChoices(typeof(ResponseCurve), responseCurveTemplates);
            curveDropDown.SetValueWithoutNotify(null);
        }

        private void UpdateResponseCurveWindow()
        {
            if (responseCurveWindow == null) return;
            rcWindowDisposable.Clear();
            responseCurveWindow.ResponseCurveViewModel.OnResponseCurveChanged
                .Subscribe(curve => onResponseCurveChanged.OnNext(curve))
                .AddTo(rcWindowDisposable);
            
            if (responseCurve != null)
            {
                responseCurveWindow.UpdateUi(responseCurve,true,ownerName);
            }
        }

        ~ResponseCurveMinimizedViewModel()
        {
            disposable.Clear();
            rcDisposable.Clear();
            rcWindowDisposable.Clear();
        }
    }