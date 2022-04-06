using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ConsiderationModifier: Consideration
{
    public ConsiderationModifier(): base()
    {
        IsModifier = true;
    }

    protected ConsiderationModifier(ConsiderationModifier original) : base(original)
    {
    }
}