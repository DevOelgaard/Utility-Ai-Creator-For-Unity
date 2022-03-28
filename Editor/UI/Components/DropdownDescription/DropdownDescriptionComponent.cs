using System;
using UnityEngine.UIElements;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using UniRxExtension;
using MoreLinq;

public class DropdownDescriptionComponent<T> : VisualElement where T : IIdentifier
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private DropdownField dropDown;
    private Label description;
    private ReactiveList<T> elements;
    public IObservable<T> OnDropdownValueChanged => onDropdownValueChanged;
    private Subject<T> onDropdownValueChanged = new Subject<T>();

    public DropdownDescriptionComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        dropDown = root.Q<DropdownField>("Elements-DropdownField");
        dropDown.RegisterCallback<ChangeEvent<string>>(evt => ValueChanged(evt.newValue));

        description = root.Q<Label>("DescriptionText-Label");
    }

    internal void UpdateUi(ReactiveList<T> elements, string labelName, string initialValue)
    {
        subscriptions.Clear();
        this.elements = elements;
        this.elements
            .OnValueChanged
            .Subscribe(_ => UpdateDropdown())
            .AddTo(subscriptions);

        dropDown.label = labelName;
        SetValue(initialValue);
        UpdateDropdown();
    }

    internal void AddElements(List<T> elements)
    {
        this.elements.Add(elements);
        //foreach(var e in elements)
        //{
        //    this.elements.Add(e);
        //}

        UpdateDropdown();
    }



    private void ValueChanged(string newValue)
    {
        var element = GetElement(newValue);
        if (element != null)
        {
            onDropdownValueChanged.OnNext(element);
            description.text = element.GetDescription();
        }
    }

    internal T GetElement(string name)
    {
        return elements.Values.FirstOrDefault(e => e.GetName() == name);
    }

    private void UpdateDropdown()
    {
        var value = dropDown.value;
        dropDown.choices.Clear();
        if (elements.Values.Count <= 0)
        {
            return;
        }
        SetValue(value);
    }

    private void SetValue(string name)
    {
        var element = elements.Values.FirstOrDefault(e => e.GetName() == name);
        if (elements == null)
        {
            dropDown.value = null;
            description.text = "";
        }
        else
        {
            dropDown.value = name;
            description.text = name;
            onDropdownValueChanged.OnNext(element);
        }
    }

    ~DropdownDescriptionComponent()
    {
        subscriptions.Clear();
    }

}
