using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;
using UnityEditor.UIElements;

internal abstract class SplitViewWindowDropDownSelection<T> : EditorWindow
{
    private readonly CompositeDisposable elementNameUpdatedSub = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftContainer;
    private VisualElement rightContainer;
    private VisualElement buttonContainer;
    private RightPanelComponent<T> rightPanelComponent;

    private DropdownField identifierDropdown;

    private IDisposable elementChangedSub;
    private IDisposable agentTypesChangedSub;
    private IDisposable agentCollectionUpdatedSub;
    private T selectedElement;
    private ReactiveList<T> elements;
    private int SelectedIndex => elements.Values.IndexOf(selectedElement);
    private AgentManager agentManager => AgentManager.Instance;
    private StyleSheet buttonSelectedStyle;
    protected Toolbar ToolbarTop;
    public void CreateGUI()
    {
        root = rootVisualElement;
        var treeAsset = AssetService.GetVisualTreeAsset("SplitViewWindowDropDownSelection");
        treeAsset.CloneTree(root);

        leftContainer = root.Q<VisualElement>("LeftContainer");
        rightContainer = root.Q<VisualElement>("RightContainer");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");

        identifierDropdown = root.Q<DropdownField>("AgentType-Dropdown");
        buttonSelectedStyle = StylesService.GetStyleSheet("ButtonSelected");
        ToolbarTop = root.Q<Toolbar>("ToolbarTop");

        rightPanelComponent = GetRightPanelComponent();
        rightContainer.Add(rightPanelComponent);

        agentTypesChangedSub = agentManager
            .AgentIdentifiersUpdated
            .Subscribe(_ => InitDropDown());

        InitDropDown();

        UpdateLeftPanel();

        agentCollectionUpdatedSub = agentManager
            .AgentsUpdated
            .Subscribe(agent =>
            {
                if (agent.TypeIdentifier == identifierDropdown.value)
                {
                    UpdateLeftPanel();
                }
            });


        root.RegisterCallback<KeyDownEvent>(key =>
        {
            if (key.keyCode == KeyCode.UpArrow && key.ctrlKey)
            {
                SelectElementAtIndex(0);
            }
            else if (key.keyCode == KeyCode.UpArrow)
            {
                SelectElementAtIndex(SelectedIndex - ConstsEditor.Logger_StepSize);
            }
            else if (key.keyCode == KeyCode.DownArrow && key.ctrlKey)
            {
                SelectElementAtIndex(elements.Count - 1);
            }
            else if (key.keyCode == KeyCode.DownArrow)
            {
                SelectElementAtIndex(SelectedIndex + ConstsEditor.Logger_StepSize);
            }

            KeyPressed(key);
        });

        Init();
    }

    protected virtual void Init() { }

    private void SelectElementAtIndex(int index)
    {

        if (index < 0)
        {
            index = 0;
        } else if (index >= elements.Count)
        {
            index = elements.Count - 1;
        }
        SelectedElement = elements.Values[index];
    }

    protected abstract RightPanelComponent<T> GetRightPanelComponent();

    private void InitDropDown()
    {
        identifierDropdown.choices = agentManager.AgentIdentifiers;
        // identifierDropdown.choices.Add("Demo");
        identifierDropdown.label = "Agent Types";
        if (agentManager.AgentIdentifiers.Count > 0)
        {
            identifierDropdown.value = identifierDropdown.choices.First();
        }
        else
        {
            identifierDropdown.value = "No agents found in scene";
        }

        identifierDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            UpdateLeftPanel(evt.newValue);
        });
    }

    protected abstract ReactiveList<T> GetLeftPanelElements(string identifier);

    private void UpdateLeftPanel(string identifier = null)
    {
        identifier ??= identifierDropdown.value;
        elements = GetLeftPanelElements(identifier);

        elementChangedSub?.Dispose();
        elementChangedSub = elements
            .OnValueChanged
            .Subscribe(LoadElements);

        LoadElements(elements.Values);
    }

    private void LoadElements(List<T> newElements)
    {
        SelectedElement = default(T);
        buttonContainer.Clear();
        elementNameUpdatedSub?.Clear();

        foreach (var e in newElements)
        {
            var button = new Button
            {
                text = GetNameFromElement(e),
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                SelectedElement = e;
            });

            buttonContainer.Add(button);
        }
    }

    protected virtual void KeyPressed(KeyDownEvent key)
    {

    }

    protected abstract string GetNameFromElement(T element);

    private void SelectedElementChanged()
    {
        rightPanelComponent.UpdateUi(SelectedElement);
        if (elements.Count <= 0) return;
        var buttons = buttonContainer.Query<Button>().ToList();
        buttons.ForEach(b => b.styleSheets.Remove(buttonSelectedStyle));

        var button = buttonContainer.Query<Button>().AtIndex(SelectedIndex);
        if (button == null) return;
        button.styleSheets.Add(buttonSelectedStyle);
    }

    private T SelectedElement
    {
        get => selectedElement;
        set
        {
            selectedElement = value;
            SelectedElementChanged();
        }
    }

    void ClearSubscriptions()
    {
        elementChangedSub?.Dispose();
        agentTypesChangedSub?.Dispose();
        agentCollectionUpdatedSub?.Dispose();
        elementNameUpdatedSub.Clear();
    }

    private void OnDestroy()
    {
        WindowOpener.windowPosition = this.position;
        // var persistenceAPI = PersistenceAPI.Instance;
        // UasTemplateService.Instance.Save();
        //persistenceAPI.SaveObjectPath(UASTemplateService.Instance, Consts.File_UASTemplateService_AutoSave);
        ClearSubscriptions();
    }

    ~SplitViewWindowDropDownSelection()
    {
        ClearSubscriptions();
    }
}