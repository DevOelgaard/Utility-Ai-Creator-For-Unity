using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;

public class IntegerFieldMinMax: IntegerField
{
    public int min = int.MinValue;

    public int max = int.MaxValue;

    public IntegerFieldMinMax()
    {
    }

    public IntegerFieldMinMax(int maxLength) : base(maxLength)
    {
    }

    public IntegerFieldMinMax(string label, int maxLength = -1) : base(label, maxLength)
    {
    }

    public override int value 
    { 
        get => base.value;
        set
        {
            if (value < min || value > max) return;
             base.value = value;
        }
    }


}
