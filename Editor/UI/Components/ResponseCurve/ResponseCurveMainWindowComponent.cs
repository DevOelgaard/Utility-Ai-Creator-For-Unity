using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveMainWindowComponent : AiObjectComponent 
{
    private ResponseCurveLCComponent responseCurveLCComponent;
    public ResponseCurveMainWindowComponent() : base()
    {
        responseCurveLCComponent = new ResponseCurveLCComponent();
        Body.Clear();
        Body.Add(responseCurveLCComponent);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var m = model as ResponseCurve;
        responseCurveLCComponent.UpdateUi(m);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal ResponseCurve");

    }
}
