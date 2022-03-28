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
    private CompositeDisposable elementNameUpdatedSub = new CompositeDisposable();

    protected VisualElement Root;
    private VisualElement leftContainer;
    private VisualElement rightContainer;
    private VisualElement buttonContainer;
    //private DebuggerComponent debuggerComponent;
    private RightPanelComponent<T> rightPanelComponent;

    private DropdownField identifierDropdown;

    private IDisposable elementChangedSub;
    private IDisposable agentTypesChangedSub;
    private IDisposable agentCollectionUpdatedSub;
    private T selectedElement;
    private ReactiveList<T> elements;
    private int selectedIndex => elements.Values.IndexOf(selectedElement);
    private AgentManager agentManager => AgentManager.Instance;
    private StyleSheet buttonSelectedStyle;
    protected Toolbar ToolbarTop;
    public void CreateGUI()
    {
        Root = rootVisualElement;
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset("SplitViewWindowDropDownSelection");
        treeAsset.CloneTree(Root);

        leftContainer = Root.Q<VisualElement>("LeftContainer");
        rightContainer = Root.Q<VisualElement>("RightContainer");
        buttonContainer = Root.Q<VisualElement>("ButtonContainer");

        identifierDropdown = Root.Q<DropdownField>("AgentType-Dropdown");
        buttonSelectedStyle = StylesService.GetStyleSheet("ButtonSelected");
        ToolbarTop = Root.Q<Toolbar>("ToolbarTop");

        rightPanelComponent = GetRightPanelComponent();
        rightContainer.Add(rightPanelComponent);

        agentTypesChangedSub = agentManager
            .AgentTypesUpdated
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


        Root.RegisterCallback<KeyDownEvent>(key =>
        {
            if (key.keyCode == KeyCode.UpArrow && key.ctrlKey)
            {
                SelectElementAtIndex(0);
            }
            else if (key.keyCode == KeyCode.UpArrow)
            {
                SelectElementAtIndex(selectedIndex - ConstsEditor.Logger_StepSize);
            }
            else if (key.keyCode == KeyCode.DownArrow && key.ctrlKey)
            {
                SelectElementAtIndex(elements.Count - 1);
            }
            else if (key.keyCode == KeyCode.DownArrow)
            {
                SelectElementAtIndex(selectedIndex + ConstsEditor.Logger_StepSize);
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
        identifierDropdown.choices.Add("Demo");
        identifierDropdown.label = "Agent Types";
        if (agentManager.AgentIdentifiers.Count > 0)
        {
            identifierDropdown.value = identifierDropdown.choices.First();
        }
        else
        {
            identifierDropdown.value = "No agents found";
        }

        identifierDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            UpdateLeftPanel(evt.newValue);
        });
    }

    protected abstract ReactiveList<T> GetLeftPanelElements(string identifier);

    private void UpdateLeftPanel(string identifier = null)
    {
        if (identifier == null)
        {
            identifier = identifierDropdown.value;
        }
        elements = GetLeftPanelElements(identifier);

        elementChangedSub?.Dispose();
        elementChangedSub = elements
            .OnValueChanged
            .Subscribe(values =>
            {
                LoadElements(values);
            });

        LoadElements(elements.Values);
    }

    private void LoadElements(List<T> elements)
    {
        SelectedElement = default(T);
        buttonContainer.Clear();
        elementNameUpdatedSub?.Clear();

        foreach (var e in elements)
        {
            var button = new Button();
            button.text = GetNameFromElement(e);
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
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
        rightPanelComponent.UpateUi(SelectedElement);
        if (elements.Count <= 0) return;
        var buttons = buttonContainer.Query<Button>().ToList();
        buttons.ForEach(b => b.styleSheets.Remove(buttonSelectedStyle));

        var button = buttonContainer.Query<Button>().AtIndex(selectedIndex);
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
        WindowOpener.WindowPosition = this.position;
        var persistenceAPI = new PersistenceAPI(new JSONPersister());
        persistenceAPI.SaveObjectPath(UASTemplateService.Instance, Consts.File_UASTemplateService_AutoSave);
        ClearSubscriptions();
    }

    ~SplitViewWindowDropDownSelection()
    {
        ClearSubscriptions();
    }
}