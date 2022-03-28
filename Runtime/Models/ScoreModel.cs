using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public class ScoreModel
{
    public string Name = "";

    private float value = 0f;
    public float Value
    {
        get => value;
        set
        {
            if (this.value != value)
            {
                this.value = value;
                valueChanged.OnNext(this.value);
            }
        }
    }

    public IObservable<float> OnValueChanged => valueChanged;
    private Subject<float> valueChanged = new Subject<float>();

    public ScoreModel(string name, float value)
    {
        Name = name;
        Value = value;
    }
}
