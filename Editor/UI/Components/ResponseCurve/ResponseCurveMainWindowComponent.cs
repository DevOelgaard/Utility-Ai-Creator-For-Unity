using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class ResponseCurveMainWindowComponent : AiObjectComponent
{
    private CompositeDisposable disposables = new CompositeDisposable();
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
        var responseCurve = model as ResponseCurve;
        responseCurveLcComponent.UpdateUi(responseCurve);
        disposables.Clear();
        // responseCurve.OnFunctionsChanged
        //     .Subscribe(_ => responseCurveLcComponent.UpdateUi(responseCurve))
        //     .AddTo(disposables);
        // responseCurve.OnFunctionsChanged
        //     .Subscribe(_ => responseCurveLcComponent.UpdateUi(responseCurve))
        //     .AddTo(disposables);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal ResponseCurve");

    }
}
