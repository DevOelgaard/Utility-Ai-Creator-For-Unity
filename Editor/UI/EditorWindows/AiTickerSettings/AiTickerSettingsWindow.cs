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
using MoreLinq;

internal class AiTickerSettingsWindow: EditorWindow
{
    private CompositeDisposable disposables = new CompositeDisposable();
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


    private AiTicker aiTicker;

    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
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
        aiTicker = AiTicker.Instance;
        modes.Init(AiTickerMode.Unrestricted);
        modes.value = aiTicker.Settings.TickerMode.Name;
        modes.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {
            aiTicker.SetTickerMode((AiTickerMode)evt.newValue);
        });

        aiTicker.Settings.OnTickerModeChanged
            .Subscribe(tickerMode => LoadTicker(tickerMode))
            .AddTo(disposables);

        startButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Start();
        });

        stopButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Stop();
        });

        reloadButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            aiTicker.Reload();
        });

        LoadTicker(aiTicker.Settings.TickerMode);
        autoRunToggle.value = aiTicker.Settings.AutoRun;
        autoRunToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            aiTicker.Settings.AutoRun = evt.newValue;
        });

        tickCount.text = aiTicker.TickCount.ToString();

        aiTicker.OnTickCountChanged
            .Subscribe(value => tickCount.text = value.ToString())
            .AddTo(disposables);



    }

    private void LoadTicker(TickerMode tickerMode)
    {
        body.Clear();
        description.text = "";
        if (tickerMode == null) return;

        tickerMode.Parameters
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
            tickerModeSub = AiTicker.Instance.OnTickComplete
                .Subscribe(_ =>
                {
                    info2Value.text = cast.TickedAgentsThisFrame.ToString();
                });
        }
        description.text = tickerMode.Description;
    }

    private void OnDestroy()
    {
        WindowOpener.WindowPosition = this.position;
        aiTicker.Save();
        disposables.Clear();
        tickerModeSub?.Dispose();
    }

}