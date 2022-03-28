using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal interface IVisualElementPoolObject<T>
{
    internal void Hide();
    internal void Display(T element);
}
