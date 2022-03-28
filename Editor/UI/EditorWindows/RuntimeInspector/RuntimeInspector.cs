using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UnityEditor.UIElements;

internal class RunTimeInspector : SplitViewWindowDropDownSelection<IAgent>
{
    protected override void Init()
    {
        UASTemplateService.Instance.LoadAutoSave();
        var debugMenu = new ToolbarMenu();
        debugMenu.text = "Debug";

        debugMenu.menu.AppendAction("Timer", _ =>
        {
            TimerService.Instance.DebugLogTime();
            InstantiaterService.Instance.DebugStuff();
        });

        debugMenu.menu.AppendAction("Reset Timer", _ =>
        {
            TimerService.Instance.Reset();
            InstantiaterService.Instance.Reset();
        });

        ToolbarTop.Add(debugMenu);
    }


    protected override ReactiveList<IAgent> GetLeftPanelElements(string identifier)
    {
        return AgentManager.Instance.GetAgentsByIdentifier(identifier);
    }

    protected override string GetNameFromElement(IAgent element)
    {
        return element.Model.Name;
    }

    protected override RightPanelComponent<IAgent> GetRightPanelComponent()
    {
        return new AgentComponent();
    }
}