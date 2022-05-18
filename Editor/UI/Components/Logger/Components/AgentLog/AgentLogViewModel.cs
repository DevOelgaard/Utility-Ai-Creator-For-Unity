using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLogViewModel : AiObjectLogViewModel
{
    internal AiLogViewModel aiLogViewModel;

    public AgentLogViewModel(): base()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        aiLogViewModel = new AiLogViewModel();
        root.Add(aiLogViewModel);
    }

    
    protected override void UpdateUiInternal(AiObjectLog aiObjectLog)
    {
        var a = aiObjectLog as AgentLog;
        aiLogViewModel.UpdateUi(a.Ai);
    }

    internal override void SetColor()
    {
        aiLogViewModel.SetColor();
    }

    internal override void ResetColor()
    {
        aiLogViewModel.ResetColor();
    }
}