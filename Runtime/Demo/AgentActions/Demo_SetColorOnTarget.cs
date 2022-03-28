using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_SetColorOnTarget : AgentAction
{
    public Demo_SetColorOnTarget()
    {
        HelpText = "The parent must set a " + AiContextKey.CurrentTargetGameObject + " for it to act on";
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Set OnStart", true),
            new Parameter("Color OnStart", Color.white),
            new Parameter("Set OnGoing", true),
            new Parameter("Color OnGoing", Color.blue),
            new Parameter("Set OnEnd", true),
            new Parameter("Color OnEnd", Color.black),
        };
    }

    public override void OnStart(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[0].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[1].Value);
        }
    }

    public override void OnGoing(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[2].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[3].Value);
        }
    }

    public override void OnEnd(AiContext context)
    {
        base.OnGoing(context);
        if ((bool)Parameters[4].Value)
        {
            var targetRenderer = GetTargetRenderer(context);
            targetRenderer.material.SetColor("_Color", (Color)Parameters[5].Value);
        }
    }

    private Renderer GetTargetRenderer(AiContext context)
    {
        var address = context.LastSelectedDecision.GetContextAddress(context);
        //Debug.Log("Address: " + address);
        var target = context.GetContext<GameObject>((address + AiContextKey.CurrentTargetGameObject));
        return target.GetComponent<Renderer>();
    }
}
