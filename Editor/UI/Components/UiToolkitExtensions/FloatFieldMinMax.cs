using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;

internal class FloatFieldMinMax: FloatField
{
    public float Min = float.MinValue;
    public float Max = float.MaxValue;

    public FloatFieldMinMax()
    {
    }

    public FloatFieldMinMax(int maxLength) : base(maxLength)
    {
    }

    public FloatFieldMinMax(string label, int maxLength = -1) : base(label, maxLength)
    {
    }

    public override float value
    {
        get => base.value;
        set
        {
            if (value < Min || value > Max) return;
            base.value = value;
        }
    }
}