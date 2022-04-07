using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Error_ResponseFunction : ResponseFunction
{
    public Error_ResponseFunction()
    {
        Name = "Error";
    }

    protected override float CalculateResponseInternal(float x)
    {
        return -1;
    }
}
