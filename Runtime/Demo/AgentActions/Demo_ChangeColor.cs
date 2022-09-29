using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_ChangeColor : AgentAction
{
    private int currentColorIndex = 0;
    private int maxColorIndex = 3;
    private Color Color0 => ParameterContainer.GetParamColor("Color 0").Value;
    private Color Color1 => ParameterContainer.GetParamColor("Color 1").Value;
    private Color Color2 => ParameterContainer.GetParamColor("Color 2").Value;
    private Color Color3 => ParameterContainer.GetParamColor("Color 3").Value;
    public Demo_ChangeColor() : base()
    {
        ParameterContainer.AddParameter("Color 0", Color.red);
        ParameterContainer.AddParameter("Color 1", Color.white);
        ParameterContainer.AddParameter("Color 2", Color.blue);
        ParameterContainer.AddParameter("Color 3", Color.black);
    }
    
    public override void OnStart(IAiContext context)
    {
        ChangeColor(context);
    }

    public override void OnGoing(IAiContext context)
    {
        ChangeColor(context);
    }

    private void ChangeColor(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"ChangeColor");
        var targetRenderer = GetTargetRenderer(context);
        var newColor = Color0;
        if (currentColorIndex == 0)
        {
            newColor = Color0;
        } else if (currentColorIndex == 1)
        {
            newColor = Color1;
        }else if (currentColorIndex == 2)
        {
            newColor = Color2;
        }else if (currentColorIndex == 3)
        {
            newColor = Color3;
        }
        targetRenderer.material.SetColor("_Color", newColor);
        currentColorIndex++;
        if (currentColorIndex > maxColorIndex)
        {
            currentColorIndex = 0;
        }
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"ChangeColor");
    }

    private Renderer GetTargetRenderer(IAiContext context)
    {
        var agent = context.Agent as AgentMono;
        var target = agent.gameObject;
        return target.GetComponent<Renderer>();
    }
}
