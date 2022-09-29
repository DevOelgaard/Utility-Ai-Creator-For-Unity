using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

internal class AiTickerSettingsWindow: EditorWindow
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable tickerModeSub;
    private VisualElement root;
    private EnumField modes;
    private VisualElement header;
    private VisualElement body;
    private VisualElement footer;
    private HelpBox description;
    private Button startButton;
    private Button stopButton;
    private Button reloadButton;
    private Label tickCount;
    private Label info2Label;
    private Label info2Value;
    private Toggle autoRunToggle;


    private UaiTicker uaiTicker;

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        modes = root.Q<EnumField>("Mode-EnumField");
        header = root.Q<VisualElement>("Header");
        body = root.Q<VisualElement>("Body");
        footer = root.Q<VisualElement>("Footer");
        startButton = root.Q<Button>("StartButton");
        stopButton = root.Q<Button>("StopButton");
        reloadButton = root.Q<Button>("ReloadButton");
        tickCount = root.Q<Label>("TickCountValue-Label");
        autoRunToggle = root.Q<Toggle>("AutoRun-Toggle");
        info2Label = root.Q<Label>("Info2Id-Label");
        info2Value = root.Q<Label>("Info2Value-Label");

        description = new HelpBox("",HelpBoxMessageType.Info);
        header.Add(description);
        uaiTicker = UaiTicker.Instance;
        modes.Init(UaiTickerMode.Unrestricted);
        modes.value = uaiTicker.Settings.CurrentTickerMode.Name;
        
        LoadTicker(uaiTicker.Settings.CurrentTickerMode);
        tickCount.text = uaiTicker.TickCount.ToString();

        
        modes.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {
            uaiTicker.SetTickerMode((UaiTickerMode)evt.newValue);
        });

        uaiTicker.Settings.OnTickerModeChanged
            .Subscribe(LoadTicker)
            .AddTo(disposables);

        startButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uaiTicker.Start();
        });

        stopButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uaiTicker.Stop();
        });

        reloadButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uaiTicker.InitSettings();
        });

        autoRunToggle.value = uaiTicker.Settings.AutoRun;
        autoRunToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            uaiTicker.Settings.AutoRun = evt.newValue;
        });


        uaiTicker.OnTickCountChanged
            .Subscribe(value => tickCount.text = value.ToString())
            .AddTo(disposables);
    }

    private void LoadTicker(TickerMode tickerMode)
    {
        body.Clear();
        description.text = "";
        if (tickerMode == null) return;

        tickerMode.ParameterContainer.Parameters
            .ToList()
            .ForEach(p =>
            {
                var pC = new ParameterComponent();
                pC.UpdateUi(p);
                body.Add(pC);
            });

        tickerModeSub?.Dispose();
        if (tickerMode.GetType() == typeof(TickerModeDesiredFrameRate))
        {
            var cast = tickerMode as TickerModeDesiredFrameRate;
            info2Label.text = "FPS: ";
            info2Value.text = "0";
            tickerModeSub = cast.OnLastFrameRateChanged
                .Subscribe(fps => info2Label.text = fps.ToString());
        } else if (tickerMode.GetType() == typeof(TickerModeTimeBudget))
        {
            var cast = tickerMode as TickerModeTimeBudget;
            info2Label.text = "TPS: ";
            info2Value.text = "0";
            tickerModeSub = UaiTicker.Instance.OnTickComplete
                .Subscribe(_ =>
                {
                    info2Value.text = cast.TickedAgentsThisFrame.ToString();
                });
        }
        description.text = tickerMode.Description;
    }

    private void OnDestroy()
    {
        WindowOpener.windowPosition = this.position;
        disposables.Clear();
        tickerModeSub?.Dispose();
    }

}