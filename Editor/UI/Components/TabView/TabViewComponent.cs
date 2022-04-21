using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class TabViewComponent : VisualElement
{
    private readonly List<Button> tabs = new List<Button>();
    private readonly List<VisualElement> contents = new List<VisualElement>();

    private readonly StyleSheet selectedStyle;
    private readonly VisualElement tabContainer;
    private readonly VisualElement content;
    private int selectedIndex = -1;

    public TabViewComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        tabContainer = root.Q<VisualElement>("Tabs");
        content = root.Q<VisualElement>("TabContent");
        selectedStyle = StylesService.GetStyleSheet("ButtonSelected");
    }

    internal Button AddTabGroup(string tabTitle, VisualElement content, int elementCount = -1)
    {
        var tab = new Button();
        tab.text = tabTitle;
        tab.style.flexGrow = 1;

        tabs.Add(tab);
        tabContainer.Add(tab);

        contents.Add(content);
        this.content.Add(content);
        if (selectedIndex < 0)
        {
            Select(0);
        } else
        {
            Deselect(tabs.IndexOf(tab));
        }

        tab.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (selectedIndex == tabs.IndexOf(tab))
            {
                Deselect(IndexOf(tab));
            } else
            {
                Deselect(selectedIndex);
                Select(tabs.IndexOf(tab));
            }
        });

        return tab;
    }

    private void Deselect(int index)
    {
        //if (index != selectedIndex) return;
        if (contents.Count <= index || index < 0) return;
        contents[index].style.display = DisplayStyle.None;
        tabs[index].styleSheets.Remove(selectedStyle);
    }

    private void Select(int index)
    {
        if (index == selectedIndex) return;

        selectedIndex = index;
        contents[index].style.display = DisplayStyle.Flex;
        tabs[index].styleSheets.Add(selectedStyle);

    }
}