using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using UnityEditor.UIElements;
using System.IO;

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
    private PersistenceAPI persistenceAPI => PersistenceAPI.Instance;

    private Dictionary<AiObjectModel, AiObjectComponent> componentsByModels = new Dictionary<AiObjectModel, AiObjectComponent>();
    private Toolbar toolbar;
    private bool autoSave = true;
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
            addElementPopup.SetValueWithoutNotify(null);
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


    void OnEnable()
    {
        autoSave = true;
        UASTemplateService.Instance.LoadCurrentProject();
        var mws = MainWindowService.Instance;
        mws.OnUpdateStateChanged
            .Subscribe(state =>
            {
                var projectName = ProjectSettingsService.Instance.GetCurrentProjectName();
                this.titleContent.text = state ? projectName + " - Loading" : projectName;
            })
            .AddTo(disposables);
        mws.Start();
    }

    private void InitToolbarFile()
    {
        var menu = new ToolbarMenu();
        menu.text = "File";

        menu.menu.AppendAction("New Project", _ =>
        {
            uASTemplateService.Save();
            ProjectSettingsService.Instance.CreateProject();
            uASTemplateService.LoadCurrentProject();
            UpdateLeftPanel();
            rightPanel.Clear();

        });

        menu.menu.AppendAction("Save", _ =>
        {
            //ProjectSettingsService.Instance.CreateProject();
            uASTemplateService.Save();
        });

        menu.menu.AppendAction("Save As", _ =>
        {
            //uASTemplateService.Save();
            ProjectSettingsService.Instance.CreateProject();
            uASTemplateService.Save();
        });

        menu.menu.AppendAction("Open Project", _ =>
        {
            uASTemplateService.Save();
            ProjectSettingsService.Instance.LoadProject();
            uASTemplateService.LoadCurrentProject();
            UpdateLeftPanel();
            rightPanel.Clear();
        });

        menu.menu.AppendAction("Export File(s)", _ =>
        {
            var saveObjects = new List<RestoreAble>();
            selectedObjects.ForEach(pair => saveObjects.Add(pair.Key));
            persistenceAPI.SaveObjectsPanel(saveObjects);
        });

        menu.menu.AppendAction("Import File(s)", _ =>
        {
            var s = persistenceAPI.LoadFilePanel<RestoreState>(Consts.FileExtensionsFilters);
            s.LoadedObject.FolderLocation = Path.GetDirectoryName(s.Path) + @"\";
            if(s == null)
            {
                return;
            }
            var t = s.StateType;

            var toCollection = uASTemplateService.GetCollection(s.ModelType);
            var restored = RestoreAble.Restore(s.LoadedObject, s.ModelType);
            toCollection.Add(restored as AiObjectModel);

        });

        menu.menu.AppendAction("Load Autosave", _ =>
        {
            uASTemplateService.LoadCurrentProject();
            UpdateLeftPanel();
        });

        menu.menu.AppendAction("Save Backup", _ =>
        {
            uASTemplateService.Save(true);
        });

        menu.menu.AppendAction("Load Backup", _ =>
        {
            uASTemplateService.LoadCurrentProject(true);
        });

        menu.menu.AppendAction("Close", _ =>
        {
            //var wnd = GetWindow<TemplateManager>();
            this.Close();
        });

        menu.menu.AppendAction("Close No Save", _ =>
        {
            autoSave = false;
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
            if (componentsByModels.ContainsKey(element))
            {
                var component = componentsByModels[element];
                rightPanel.Remove(component);
                componentsByModels.Remove(element);
            }
        }

        SelectedModel = null;
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
            if(currentMainWindowComponent != null)
            {
                currentMainWindowComponent.style.display = DisplayStyle.None;
            }

            UpdateLeftPanel(evt.newValue);
        });
    }

    public void UpdateLeftPanel(string label = "")
    {
        titleContent.text = ProjectSettingsService.Instance.GetCurrentProjectName();

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
            .OrderBy(t => t)
            .ToList();

        LoadModels(models.Values);
        UpdateButtons();
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
        uASTemplateService.Save();
        ClearSubscriptions();
    }

    private void OnDisable()
    {
        OnClose();
    }

    void OnDestroy()
    {
        OnClose();
    }

    private void OnClose()
    {
        WindowOpener.WindowPosition = this.position;
        if (autoSave)
        {
            uASTemplateService.Save();
            ProjectSettingsService.Instance.SaveSettings();
        }
        ClearSubscriptions();
    }

    private void ClearSubscriptions()
    {
        activeCollectionChangedSub?.Dispose();
        modelsChangedSubsciptions.Clear();
        disposables.Clear();
    }
}