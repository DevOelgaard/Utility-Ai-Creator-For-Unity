using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

internal class LineChartButton: CurveField
{

    protected override void ExecuteDefaultAction(EventBase evt)
    {
    }

    protected override void ExecuteDefaultActionAtTarget(EventBase evt)
    {
    }

    internal void UpdateUi(ResponseCurve responseCurve)
    {
        value = new AnimationCurve(GetKeyframes(responseCurve));
    }

    private Keyframe[] GetKeyframes(ResponseCurve responseCurve)
    {
        var points = new List<Keyframe>();
        var stepSize = (responseCurve.MaxX - responseCurve.MinX) / ConstsEditor.ResponseCurve_Steps;
        for (var i = 0; i <= ConstsEditor.ResponseCurve_Steps; i++)
        {
            var x = i * stepSize + responseCurve.MinX;
            var y = responseCurve.CalculateResponse(x);
            points.Add(new Keyframe(x, y));
        }
        return points.ToArray();
    }
}
