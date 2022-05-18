using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_DebugLogContextAddress : AgentAction
{
    public Demo_DebugLogContextAddress() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
        };
    }
    
    public override void OnStart(IAiContext context)
    {
        Print(context);
    }

    public override void OnGoing(IAiContext context)
    {
        Print(context);
    }

    private void Print(IAiContext context)
    {
        
        //TODO make sure entire address is printed
        DebugService.Log(ContextAddress.Address,this);
    }
}