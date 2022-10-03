using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class DropdownContainerViewModel<T> : VisualElement where T : UtilityContainerSelector
{
    private readonly DropdownField dropdown;
    private List<T> utilityContainerSelectors;

    private readonly VisualElement elementContainer;
    private readonly UcsViewModel ucsViewModel;
    private T selectedObject;

    internal IObservable<T> OnSelectedObjectChanged => onSelectedObjectChanged;
    private readonly Subject<T> onSelectedObjectChanged = new Subject<T>();

    public DropdownContainerViewModel(string title)
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);

        dropdown = root.Q<DropdownField>("Dropdown");
        elementContainer = root.Q<VisualElement>("ElementContainer");
        ucsViewModel = new UcsViewModel();
        elementContainer.Add(ucsViewModel);

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
        ucsViewModel.UpdateUi(ucs);
        onSelectedObjectChanged.OnNext(ucs);
    }

    internal T GetSelectedElement()
    {
        return selectedObject;
    }
}
