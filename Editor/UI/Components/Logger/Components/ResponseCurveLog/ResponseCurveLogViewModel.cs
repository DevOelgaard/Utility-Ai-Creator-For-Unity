using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;


internal class ResponseCurveLogViewModel : AiObjectLogViewModel
{
    //private ResponseFunctionLogPool pool;
    private readonly LogComponentPool<ResponseFunctionLogViewModel> responseFunctionPool;
    public ResponseCurveLogViewModel() : base()
    {
        var root = AssetService.GetTemplateContainer(this.GetType().FullName);
        Body.Add(root);
        responseFunctionPool = new LogComponentPool<ResponseFunctionLogViewModel>(root,false,"Response Functions",1);
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