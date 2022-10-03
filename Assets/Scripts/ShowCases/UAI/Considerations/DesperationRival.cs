using System.Collections.Generic;
using ShowCases.UAI;
using UnityEngine;

public class DesperationRival: Desperation
{
    public DesperationRival()
    {
        Description = "Returns the archers desperation. Relative to his rival";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;
        ParameterContainer.AddParameter("Rival",Archers.One);
    }
    

    // protected override List<Parameter> GetParameters()
    // {
    //     return new List<Parameter>()
    //     {
    //         new ParameterEnum("Rival", Archers.One)
    //     };
    // }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        var rivalName = (Archers)ParameterContainer.GetParamEnum("Rival").Value;
        // var rivalName = (Archers)GetParameter("Rival").Value;
        var rival = GameObject.Find(rivalName.ToString()).GetComponent<Archer>();
        var gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        return gameHandler.GetDesperation(archer, rival);
    }
}