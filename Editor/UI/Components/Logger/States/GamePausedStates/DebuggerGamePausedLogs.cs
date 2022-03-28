//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UIElements;

//internal class DebuggerGamePausedLogs : GamePausedNestedState
//{
//    private AiLogComponent aiLogComponent;
//    private AiLogComponent AiLogComponent
//    {
//        get
//        {
//            if (aiLogComponent == null)
//            {
//                aiLogComponent = Root.Query<AiLogComponent>().First();
//            }
//            return aiLogComponent;
//        }
//    }
//    public DebuggerGamePausedLogs(TemplateContainer root, DebuggerComponent debuggerComponent) 
//        : base(root, debuggerComponent)
//    {
//        He = new HelpBox("AI logs for the selected agent at this tick", HelpBoxMessageType.Info);
//        Body.Add(helpBox);
//        helpBox.style.display = DisplayStyle.None;
//    }

//    internal override void OnEnter(IAgent agent)
//    {
//        base.OnEnter(agent);
//        InfoLabelLeft.text = "Ai logs";
//        RecordToggle.text = "Logs";
//        InspectAi(CurrentTick);
//    }

//    internal override void OnExit()
//    {
//        base.OnExit();
//        AgentComponent.style.display = DisplayStyle.Flex;
//        RecordToggle.text = "";
//    }

//    internal override void TickChanged(int value)
//    {
//        base.TickChanged(value);
//        InspectAi(value);
//    }


//}