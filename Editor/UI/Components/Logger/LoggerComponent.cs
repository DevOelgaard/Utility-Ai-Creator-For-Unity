using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UniRx;
using UnityEditor.UIElements;

internal class LoggerComponent : RightPanelComponent<IAgent>
{
    private CompositeDisposable disposables = new CompositeDisposable();
    
    private TemplateContainer root;
    private VisualElement Body;
    private VisualElement Footer;

    private Button backLeapButton;
    private Button backStepButton;
    private Button toggleStateButton;
    private Button forwardStepButton;
    private Button forwardLeapButton;

    private LoggerState state;

    private AgentLogComponent agentLogComponent;
    private HelpBox helpBox;

    private SliderInt tickSlider;

    private bool isPlaying => EditorApplication.isPlaying;
    private bool isPaused => EditorApplication.isPaused;
    private IAgent agent;
    internal int CurrentTick;
    private Button tickAgent;

    private Button foldoutButton;

    private bool isExpanded = false;
    

    public LoggerComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        root.styleSheets.Add(StylesService.GetStyleSheet("Logger"));
        backLeapButton = root.Q<Button>("BackLeapButton");
        backStepButton = root.Q<Button>("BackStepButton");
        toggleStateButton = root.Q<Button>("ToggleButton");
        forwardStepButton = root.Q<Button>("ForwardStepButton");
        forwardLeapButton = root.Q<Button>("ForwardLeapButton");
        Body = root.Q<VisualElement>("Body");
        Footer = root.Q<VisualElement>("Footer");
        tickSlider = root.Q<SliderInt>("Tick-Slider");
        foldoutButton = root.Q<Button>("Foldout-Button");
        helpBox = new HelpBox();
        helpBox.style.display = DisplayStyle.None;
        Body.Add(helpBox);

        agentLogComponent = new AgentLogComponent();
        Body.Add(agentLogComponent);
        agentLogComponent.style.display = DisplayStyle.None;

        tickAgent = new Button();
        tickAgent.text = ConstsEditor.Button_TickAgent_Text;
        tickAgent.name = ConstsEditor.Button_TickAgent_Name;
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (agent == null) return;
            AiTicker.Instance.TickAgent(agent);
        });
        Footer.Add(tickAgent);


        backLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.BackLeapButtonPressed();
        });

        backStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.BackStepButtonPressed();
        });

        toggleStateButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ToggleStateButtonPressed();
        });

        forwardStepButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ForwardStepButtonPressed();
        });

        forwardLeapButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            state.ForwardLeapButtonPressed();
        });
        
        tickSlider.RegisterCallback<ChangeEvent<int>>(evt =>
        {
            state.TickSliderChanged(evt.newValue);
        });

        foldoutButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            ToggleFoldStatus(isExpanded);
        });

        SetFoldButtonText();

        EditorApplication.pauseStateChanged += _ => UpdateGameState();
        
        UpdateGameState();
    }

    private void ToggleFoldStatus(bool fold)
    {
        isExpanded = !fold;
        root
            //.Query<BucketLogComponent>()
            .Query<Foldout>("LoggerFoldout")
            .ForEach(foldout => foldout.value = isExpanded);
        SetFoldButtonText();
    }

    private void SetFoldButtonText()
    {
        if (isExpanded)
        {
            foldoutButton.text = "Collapse All";
        }
        else
        {
            foldoutButton.text = "Expand All";
        }
    }

    internal void SetState(LoggerState state)
    {
        this.state?.OnExit();
        this.state = state;
        this.state.OnEnter(agent);
    }

    internal override void UpateUi(IAgent element)
    {
        this.agent = element;
        state.UpdateUi(agent);
    }

    internal void KeyPressed(KeyDownEvent key)
    {
        state.KeyPressed(key);
    }

    private void UpdateGameState()
    {
        if (isPlaying && !isPaused)
        {
            SetState(new LoggerGameRunning(root, this));
        }
        else if (isPlaying && isPaused)
        {
            SetState(new LoggerGamePaused(root, this));
        }
        else
        {
            SetState(new LoggerGameStopped(root, this));
        }
    }

    ~LoggerComponent()
    {
        disposables.Clear();
    }
}

