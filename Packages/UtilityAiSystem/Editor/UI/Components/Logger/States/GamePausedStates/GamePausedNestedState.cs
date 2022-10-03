//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine.UIElements;

//internal abstract class GamePausedNestedState
//{
//    protected DebuggerComponent DebuggerComponent;
//    protected TemplateContainer Root;

//    protected Label InfoLabelLeft;

//    protected Button BackLeapButton;
//    protected Button BackStepButton;
//    protected Button ToggleStateButton;
//    protected Button ForwardStepButton;
//    protected Button ForwardLeapButton;

//    protected SliderInt TickSlider;

//    protected Toggle RecordToggle;

//    protected VisualElement Body;
//    private AiLogComponent aiLogComponent;
//    protected AiLogComponent AiLogComponent
//    {
//        get
//        {
//            if(aiLogComponent == null)
//            {
//                aiLogComponent = Root.Query<AiLogComponent>().First();
//            }
//            return aiLogComponent;
//        }
//    }

//    public IAgent Agent { get; protected set; }
//    protected Ai PlayAi;

//    protected int CurrentTick => TickSlider.value;

//    protected GamePausedNestedState(TemplateContainer root, DebuggerComponent debuggerComponent)
//    {
//        Root = root;
//        DebuggerComponent = debuggerComponent;
//        Init();
//    }

//    protected virtual void Init()
//    {
//        InfoLabelLeft = Root.Q<Label>("InfoLeft-Label");

//        BackLeapButton = Root.Q<Button>("BackLeapButton");
//        BackStepButton = Root.Q<Button>("BackStepButton");
//        ToggleStateButton = Root.Q<Button>("ToggleButton");
//        ForwardStepButton = Root.Q<Button>("ForwardStepButton");
//        ForwardLeapButton = Root.Q<Button>("ForwardLeapButton");

//        TickSlider = Root.Q<SliderInt>("Tick-Slider");

//        RecordToggle = Root.Q<Toggle>("Record-Toggle");

//        Body = Root.Q<VisualElement>("Body");
//    }

//    internal virtual void OnEnter(IAgent agent) 
//    {
//        UpdateAgent(agent);
//    }
//    internal virtual void OnExit() { }

//    internal virtual void BackLeapButtonPressed()
//    {

//    }

//    internal virtual void BackStepButtonPressed()
//    {
//    }

//    internal virtual void ForwardStepButtonPressed()
//    {
//    }

//    internal virtual void ForwardLeapButtonPressed()
//    {
//    }

//    internal virtual void TickChanged(int value)
//    {

//    }

//    internal virtual void UpdateAgent(IAgent agent)
//    {
//        Agent = agent;
//    }
//}
