using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;


internal class ResponseCurveLogComponent : AiObjectLogComponent
{
    //private ResponseFunctionLogPool pool;
    private LogComponentPool<ResponseFunctionLogComponent> responseFunctionPool;
    public ResponseCurveLogComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(this.GetType().FullName);
        Body.Add(root);
        responseFunctionPool = new LogComponentPool<ResponseFunctionLogComponent>(root,false,"Response Functions",1);
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectDebug)
    {
        var rc = aiObjectDebug as ResponseCurveLog;
        var logModels = new List<ILogModel>();
        foreach(var rf in rc.ResponseFunctions)
        {
            logModels.Add(rf);
        }
        responseFunctionPool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        responseFunctionPool.Hide();
    }
}