using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

internal class Demo_MoveRandom : AgentAction
{
    public Demo_MoveRandom() : base()
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
        MoveRandom(context);
    }

    public override void OnGoing(IAiContext context)
    {
        MoveRandom(context);
    }

    private void MoveRandom(IAiContext context)
    {
        var agent = context.Agent as AgentMono;
        var gO = agent.gameObject;

        var directionNumber = Random.Range(0, 4);
        var direction = Vector2.zero;
        if (directionNumber == 0)
        {
            direction = Vector2.down;
        } else if (directionNumber == 1)
        {
            direction = Vector2.up;
        }else if (directionNumber == 2)
        {
            direction = Vector2.left;
        }else if (directionNumber == 3)
        {
            direction = Vector2.right;
        }

        var range = Random.Range(1, 5);
        direction *= range;
        var movement = new Vector3(direction.x, direction.y, 0);
        gO.transform.position += movement;
    }
}