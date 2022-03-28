using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using UnityEditor.UIElements;

internal class TemplateManager : EditorWindow
{
    private IDisposable activeCollectionChangedSub;
    private CompositeDisposable disposables = new CompositeDisposable();
    private CompositeDisposable modelsChangedSubsciptions = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftPanel;
    private VisualElement elementsContainer;
    private VisualElement rightPanel;
    private VisualElement buttonContainer;
    private Button copyButton;
    private Button deleteButton;
    private Button clearButton;
    private PopupField<string> addElementPopup;
    private List<string> dropDownChoices;
    private DropdownField dropDown;
    private UASTemplateService uASTemplateService => UASTemplateService.Instance;

    private AiObjectComponent  currentMainWindowComponent;
    private AiObjectModel selectedModel;
    private PersistenceAPI persistenceAPI = new PersistenceAPI(new JSONPersister());

    private Dictionary<AiObjectModel, AiObjectComponent> componentsByModels = new Dictionary<AiObjectModel, AiObjectComponent>();
    private Toolbar toolbar;
    private AiObjectModel SelectedModel
    {
        get => selectedModel;
        set
        {
            selectedModel = value;
            UpdateButtons();
        }
    }

    internal void CreateGUI()
    {
        root = rootVisualElement;
        
        var treeAsset = AssetDatabaseService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);
        toolbar = root.Q<Toolbar>();

        dropDown = root.Q<DropdownField>("TypeDropdown");

        leftPanel = root.Q<VisualElement>("left-panel");
        elementsContainer = leftPanel.Q<VisualElement>("ButtonContainer");

        buttonContainer = root.Q<VisualElement>("Buttons");
        addElementPopup = new PopupField<string>("Add element");
        addElementPopup.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (evt.newValue == null) return;
            AddNewAiObject(evt.newValue);
            addElementPopup.value = null;
        });

        var AddElementPopupContainer = root.Q<VisualElement>("AddElementPopupContainer");
        AddElementPopupContainer.Add(addElementPopup);
        InitToolbarFile();
        InitToolbarDebug();

        rightPanel = root.Q<VisualElement>("right-panel");

        copyButton = root.Q<Button>("CopyButton");
        copyButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            CopySelectedElements();
        });

        deleteButton = root.Q<Button>("DeleteButton");
        deleteButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            DeleteSelectedElements();
        });

        clearButton = root.Q<Button>("ClearButton");
        clearButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            uASTemplateService.Reset();
            UpdateLeftPanel();
        });

        InitDropdown();
        UpdateLeftPanel();
    }

    private void InitToolbarFile()
    {
        var menu = new ToolbarMenu();
        menu.text = "File";

        menu.menu.AppendAction("Save Project", _ =>
        {
            persistenceAPI.SaveObjectPanel(uASTemplateService);
        });

        menu.menu.AppendAction("Load Project", _ =>
        {
            var uasState = persistenceAPI.LoadObjectPanel<UASTemplateServiceState>();
            uASTemplateService.Restore(uasState);
            UpdateLeftPanel();
        });

        menu.menu.AppendAction("Export File(s)", _ =>
        {
            var saveObjects = new List<RestoreAble>();
            selectedObjects.ForEach(pair => saveObjects.Add(pair.Key));
            var type = MainWindowService.Instance.GetTypeFromString(dropDown.value);
            var restoreAble = new RestoreAbleCollection(saveObjects, type);
            persistenceAPI.SaveObjectPanel(restoreAble);
        });

        menu.menu.AppendAction("Import File(s)", _ =>
        {
            var state = persistenceAPI.LoadObjectPanel<RestoreAbleCollectionState>(Consts.FileExtensions);
            if (state == null || state == default)
            {
                return;
            }
            var loadedCollection = RestoreAbleCollection.Restore<RestoreAbleCollection>(state);
            var toCollection = uASTemplateService
                .GetCollection(loadedCollection.Type);
            var castlist = loadedCollection.Models.Cast<AiObjectModel>();
            toCollection.Add(castlist);
        });

        menu.menu.AppendAction("Load Autosave", _ =>
        {
            uASTemplateService.LoadAutoSave();
            UpdateLeftPanel();
        });

        menu.menu.AppendAction("Save Backup", _ =>
        {
            uASTemplateService.AutoSave(true);
        });

        menu.menu.AppendAction("Load Backup", _ =>
        {
            uASTemplateService.LoadAutoSave(true);
        });

        menu.menu.AppendAction("Close", _ =>
        {
            //var wnd = GetWindow<TemplateManager>();
            this.Close();
        });

        toolbar.Add(menu);
    }

    private void InitToolbarDebug()
    {
        var menu = new ToolbarMenu();
        menu.text = "Debug";

        menu.menu.AppendAction("Timer Print", _ =>
        {
            TimerService.Instance.DebugLogTime();
            InstantiaterService.Instance.DebugStuff();
        });

        menu.menu.AppendAction("Timer Reset", _ =>
        {
            TimerService.Instance.Reset();
            InstantiaterService.Instance.Reset();
        });
        toolbar.Add(menu);

    }

    void OnEnable()
    {
        var mws = MainWindowService.Instance;
        mws.OnUpdateStateChanged
            .Subscribe(state =>
            {
                //var wnd = GetWindow<TemplateManager>();
                this.titleContent.text = state ? Consts.Window_TemplateManager_Name + " - Loading" : Consts.Window_TemplateManager_Name;
            })
            .AddTo(disposables);
        mws.Start();
    }

    private void UpdateButtons()
    {
        copyButton.SetEnabled(SelectedModel != null);
        deleteButton.SetEnabled(SelectedModel != null);
    }

    private void AddNewAiObject(string name)
    {
        var aiObject = AssetDatabaseService.GetInstanceOfType<AiObjectModel>(name);
        uASTemplateService.Add(aiObject);
        ModelSelected(aiObject);
    }

    private void CopySelectedElements()
    {
        var clones = new List<AiObjectModel>();
        foreach (var element in selectedObjects)
        {
            var clone = element.Key.Clone();
            clones.Add(clone);
        }
        foreach(var clone in clones)
        {
            uASTemplateService.Add(clone);
        }

        SelectedModel = clones[0];
    }

    private void DeleteSelectedElements()
    {
        var toDelete = new List<AiObjectModel>();
        foreach(var element in selectedObjects)
        {
            toDelete.Add(element.Key);
        }
        foreach(var element in toDelete)
        {
            uASTemplateService.Remove(element);
        }
        selectedModel = null;
    }

    private void InitDropdown()
    {
        dropDownChoices = new List<string> {
                Consts.Label_UAIModel, 
                Consts.Label_BucketModel, 
                Consts.Label_DecisionModel, 
                Consts.Label_ConsiderationModel,
                Consts.Label_AgentActionModel,
                Consts.Label_ResponseCurve
        };

        dropDown.label = "Categories";
        dropDown.choices = dropDownChoices;
        dropDown.value = dropDownChoices[0];
        dropDown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            rightPanel.Clear();
            UpdateLeftPanel(evt.newValue);
        });
    }

    public void UpdateLeftPanel(string label = "")
    {
        if (String.IsNullOrEmpty(label))
        {
            label = dropDown.value;
        }
        var models = uASTemplateService.GetCollection(label);
        if (models == null) return;

        activeCollectionChangedSub?.Dispose();
        activeCollectionChangedSub = models
            .OnValueChanged
            .Subscribe(values => LoadModels(values));

        MainWindowService.Instance.PreloadComponents(models);
        var type = MainWindowService.Instance.GetTypeFromString(label);
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(type);

        addElementPopup.choices = namesFromFiles
            .Where(t => !t.Name.Contains("Mock") && !t.Name.Contains("Stub"))
            .Select(t => t.Name)
            .ToList();

        LoadModels(models.Values);
        UpdateButtons();

        if (SelectedModel?.GetType() != type)
        {
            rightPanel.Clear();
        }
    }

    private void LoadModels(List<AiObjectModel> models)
    {
        SelectedModel = null;
        elementsContainer.Clear();
        buttons.Clear();
        selectedObjects.Clear();
        modelsChangedSubsciptions.Clear();

        foreach (var model in models)
        {
            var button = new Button();
            button.text = model.GetUiName();
            button.style.unityTextAlign = TextAnchor.MiddleLeft;
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                ObjectClicked(model, button, evt);
            });
            elementsContainer.Add(button);
            buttons.Add(button);
            model.OnNameChanged
                .Subscribe(newName => button.text = model.GetUiName())
                .AddTo(modelsChangedSubsciptions);
        }

        var type = MainWindowService.Instance.GetTypeFromString(dropDown.value);

    }

    private List<Button> buttons = new List<Button>();
    private List<KeyValuePair<AiObjectModel,Button>> selectedObjects = new List<KeyValuePair<AiObjectModel, Button>>();

    private void ObjectClicked(AiObjectModel model, Button button, MouseUpEvent e)
    {
        if(e.ctrlKey)
        {
            var isSelected = selectedObjects.FirstOrDefault(o => o.Key == model).Key;
            if (isSelected == null)
            {
                selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(model, button));
            } else
            {
                selectedObjects = selectedObjects.Where(o => o.Key != model).ToList();
            }
        } 
        else if (e.shiftKey)
        {
            var selectedIndex = buttons.IndexOf(button);
            var lowestSelectedIndex = int.MaxValue;
            var highestSelectedIndex = int.MinValue;
            selectedObjects
                .ForEach(pair =>
                {
                    var i = buttons.IndexOf(pair.Value);
                    if (i < lowestSelectedIndex)
                    {
                        lowestSelectedIndex = i;
                    }
                    if (i > highestSelectedIndex)
                    {
                        highestSelectedIndex = i;
                    }
                });
            if (selectedIndex < lowestSelectedIndex)
            {
                selectedObjects.Clear();
                for(var i = selectedIndex; i <= lowestSelectedIndex; i++)
                {
                    var m = uASTemplateService.GetCollection(dropDown.value).Values[i];
                    var b = buttons[i];
                    selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(m, b));
                }
            } else if (selectedIndex > highestSelectedIndex)
            {
                selectedObjects.Clear();
                for(var i = highestSelectedIndex; i <= selectedIndex; i++)
                {
                    var m = uASTemplateService.GetCollection(dropDown.value).Values[i];
                    var b = buttons[i];
                    selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(m, b));
                }
            }
        }
        else
        {
            selectedObjects.Clear();
            selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(model,button));
        }

        SelectedModel = model;
        ModelSelected(model);

        foreach (var b in buttons)
        {
            b.style.color = Color.white;
        }

        foreach(var pair in selectedObjects)
        {
            pair.Value.style.color = Color.gray;
        }
    }
    private int selectionCounter = 0;

    private void ModelSelected(AiObjectModel model)
    {
        if (model == SelectedModel)
        {
            selectionCounter++;
        } else
        {
            selectionCounter = 1;
            SelectedModel = model;
        }

        if (selectionCounter > 1)
        {
            if (currentMainWindowComponent != null)
            {
                currentMainWindowComponent.style.display = DisplayStyle.None;
            }
            if (componentsByModels.ContainsKey(model))
            {
                currentMainWindowComponent = componentsByModels[model];
                currentMainWindowComponent.style.display = DisplayStyle.Flex;
            } else
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var mvc = MainWindowService.Instance.RentComponent(model);
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "TMP Rent");
                sw.Restart();
                componentsByModels.Add(model, mvc);
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "TMP componentsByModels.Add");
                sw.Restart();
                mvc.UpdateUi(model);
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "TMP UpdateUi");
                sw.Restart();
                rightPanel.Add(mvc);
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "TMP rightPanel.Add");
                sw.Restart();

                currentMainWindowComponent = mvc;
            }
            selectionCounter = 0;
        }
    }

    ~TemplateManager()
    {
        uASTemplateService.AutoSave();
        ClearSubscriptions();
    }

    void OnDestroy()
    {
        WindowOpener.WindowPosition = this.position;
        uASTemplateService.AutoSave();
        ClearSubscriptions();
    }

    private void ClearSubscriptions()
    {
        activeCollectionChangedSub?.Dispose();
        modelsChangedSubsciptions.Clear();
        disposables.Clear();
    }
}