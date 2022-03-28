using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class LogComponentPool<T> where T: LogComponent
{
    private VisualElement root;
    internal List<T> LogComponents = new List<T>();
    private List<Foldout> foldouts = new List<Foldout>();
    private bool isFoldout;
    private bool startExpanded;
    internal LogComponentPool(VisualElement r, bool addToFoldout, string title, int initialPoolSize = 1, bool startExpanded = true, bool addMainElementToFoldout = true)
    {
        if (addMainElementToFoldout)
        {
            root = new Foldout();
            var rootCast = root as Foldout;
            rootCast.text = title;
            r.Add(root);
        }
        else
        {
            root = r;
        }

        this.isFoldout = addToFoldout;
        this.startExpanded = startExpanded;
        for(var i = 0; i < initialPoolSize; i++)
        {
            var component = (T)InstantiaterService.Instance.CreateInstance(typeof(T));

            //var component = (T)Activator.CreateInstance(typeof(T));
            LogComponents.Add(component);
            if (addToFoldout)
            {
                var foldout = new Foldout();
                foldout.name = "LoggerFoldout";
                foldout.value = startExpanded;
                root.Add(foldout);
                foldouts.Add(foldout);
                foldout.Add(component);
            } else
            {
                root.Add(component);
            }
            
            component.Hide();
        }
    }


    internal void Display(List<ILogModel> elements)
    {
        root.style.display = DisplayStyle.Flex;
        for (var i = 0; i < elements.Count; i++)
        {
            if (i >= LogComponents.Count)
            {
                var p = (T)InstantiaterService.Instance.CreateInstance(typeof(T));

                //var p = (T)Activator.CreateInstance(typeof(T));
                p.UpdateUi(elements[i]);
                p.style.display = DisplayStyle.Flex;

                LogComponents.Add(p);
                if (isFoldout)
                {
                    var foldout = new Foldout();
                    foldout.name = "LoggerFoldout";
                    foldout.value = startExpanded;
                    foldout.Add(p);
                    foldout.text = p.GetUiName();
                    foldouts.Add(foldout);
                    root.Add(foldout);
                } else
                {
                    root.Add(p);
                }
            }
            else
            {
                LogComponents[i].UpdateUi(elements[i]);
                LogComponents[i].style.display = DisplayStyle.Flex;
                if (isFoldout)
                {
                    foldouts[i].style.display = DisplayStyle.Flex;
                    foldouts[i].text = LogComponents[i].GetUiName();
                }
            }
        }

        for (var i = elements.Count; i < LogComponents.Count; i++)
        {
            LogComponents[i].Hide();
            if (isFoldout)
            {
                foldouts[i].style.display = DisplayStyle.None;
            }
        }
    }

    internal void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
