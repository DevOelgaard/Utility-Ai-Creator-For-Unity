using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLogComponent : AiObjectLogComponent
{
    internal AiLogComponent AiLogComponent;

    public AgentLogComponent(): base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        AiLogComponent = new AiLogComponent();
        root.Add(AiLogComponent);
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectLog)
    {
        var a = aiObjectLog as AgentLog;
        AiLogComponent.UpdateUi(a.Ai);
    }

    internal override void SetColor()
    {
        AiLogComponent.SetColor();
    }

    internal override void ResetColor()
    {
        AiLogComponent.ResetColor();
    }
}