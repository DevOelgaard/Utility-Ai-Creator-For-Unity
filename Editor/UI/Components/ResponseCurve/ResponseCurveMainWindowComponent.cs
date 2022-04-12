using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveMainWindowComponent : AiObjectComponent 
{
    private readonly ResponseCurveLcComponent responseCurveLcComponent;
    public ResponseCurveMainWindowComponent() : base()
    {
        responseCurveLcComponent = new ResponseCurveLcComponent();
        Body.Clear();
        Body.Add(responseCurveLcComponent);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var m = model as ResponseCurve;
        responseCurveLcComponent.UpdateUi(m);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal ResponseCurve");

    }
}
