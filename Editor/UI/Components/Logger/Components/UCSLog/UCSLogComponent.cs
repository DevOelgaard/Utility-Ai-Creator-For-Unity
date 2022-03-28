using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class UCSLogComponent : LogComponent
{
    private Label titleLabel;
    private Label nameLabel;
    private LogComponentPool<ParameterLogComponent> parameterPool;
    private UCSLog ucsLog;
    public UCSLogComponent(string title): base()
    {
        titleLabel = new Label();
        titleLabel.name = "Title-Label";
        Add(titleLabel);
        titleLabel.text = title;
        nameLabel = new Label();
        nameLabel.name = "Name-Label";
        Add(nameLabel);
        parameterPool = new LogComponentPool<ParameterLogComponent>(this, false,"Parameters", 1);
    }

    internal override string GetUiName()
    {
        return ucsLog.Name;
    }

    internal override void UpdateUi(ILogModel element)
    {
        ucsLog = (UCSLog)element;
        nameLabel.text = ucsLog.Name;

        var logModels = new List<ILogModel>();
        foreach (var p in ucsLog.Parameters)
        {
            logModels.Add(p);
        }

        parameterPool.Display(logModels);
    }
}
