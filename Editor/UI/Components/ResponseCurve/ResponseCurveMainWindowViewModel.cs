using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class ResponseCurveMainWindowViewModel : AiObjectViewModel
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private readonly ResponseCurveLcViewModel responseCurveLcViewModel;
    public ResponseCurveMainWindowViewModel() : base()
    {
        responseCurveLcViewModel = new ResponseCurveLcViewModel();
        Body.Clear();
        Body.Add(responseCurveLcViewModel);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var responseCurve = model as ResponseCurve;
        responseCurveLcViewModel.UpdateUi(responseCurve);
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
