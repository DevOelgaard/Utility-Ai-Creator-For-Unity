//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine.UIElements;

//internal class DebuggerGamePausedInspect : GamePausedNestedState
//{
//    public DebuggerGamePausedInspect(TemplateContainer root, DebuggerComponent debuggerComponent) : base(root, debuggerComponent)
//    {
//    }

//    internal override void OnEnter(IAgent agent)
//    {
//        base.OnEnter(agent);
//        InfoLabelLeft.text = "Ai Inspect";
//        RecordToggle.text = "Logs";

//        BackLeapButton.SetEnabled(true);
//        BackStepButton.SetEnabled(true);
//        ForwardStepButton.SetEnabled(true);
//        ForwardLeapButton.SetEnabled(true);
//        TickSlider.SetEnabled(true);
//    }


//    internal override void OnExit()
//    {
//        base.OnExit();
//        RecordToggle.text = "";

//    }

//    internal override void TickChanged(int value)
//    {
//        base.TickChanged(value);
//        if (value > AiLoggerService.Instance.MaxTick) // Going forward beyond logs
//        {
//            throw new NotImplementedException("Skip X frames then pause");
//            // Skip X frames then pause
//        } else // Inspecting logs
//        {
//            // Setting false to jump to logs
//            RecordToggle.value = true;
//        }
//    }


//}