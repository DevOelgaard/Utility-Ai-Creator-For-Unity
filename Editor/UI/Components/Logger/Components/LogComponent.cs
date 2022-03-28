using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal abstract class LogComponent: VisualElement
{
    internal abstract void UpdateUi(ILogModel element);

    internal virtual void Hide()
    {
        this.style.display = DisplayStyle.None;
    }

    internal abstract string GetUiName();
}
