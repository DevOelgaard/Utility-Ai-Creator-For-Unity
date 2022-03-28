using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class DropdownContainerComponent<T> : VisualElement where T : UtilityContainerSelector
{
    private DropdownField dropdown;
    private List<T> utilityContainerSelectors;

    private VisualElement elementContainer;
    private UCSComponent ucsComponent;
    private T selectedObject;

    internal IObservable<T> OnSelectedObjectChanged => onSelectedObjectChanged;
    private Subject<T> onSelectedObjectChanged = new Subject<T>();

    public DropdownContainerComponent(string title)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        dropdown = root.Q<DropdownField>("Dropdown");
        elementContainer = root.Q<VisualElement>("ElementContainer");
        ucsComponent = new UCSComponent();
        elementContainer.Add(ucsComponent);

        dropdown.label = title;

        dropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            var newUcs = utilityContainerSelectors.FirstOrDefault(ucs => ucs.GetName() == evt.newValue);
            if (newUcs != null)
            {
                SetElement(newUcs);
            }
        });
    }

    internal void UpdateUi(List<T> elements, int selectedElementIndex = 0, string title = "")
    {
        this.utilityContainerSelectors = elements;

        if (!string.IsNullOrEmpty(title))
        {
            dropdown.label = title;
        }

        dropdown.choices.Clear();
        foreach(var ucs in elements)
        {
            dropdown.choices.Add(ucs.GetName());
        }
        SetElement(utilityContainerSelectors[selectedElementIndex]);
    }

    private void SetElement(T ucs)
    {
        selectedObject = ucs;
        dropdown.SetValueWithoutNotify(ucs.GetName());

        //ucsComponent.Clear();
        ucsComponent.UpdateUi(ucs);
        onSelectedObjectChanged.OnNext(ucs);
    }

    internal T GetSelectedElement()
    {
        return selectedObject;
    }
}
