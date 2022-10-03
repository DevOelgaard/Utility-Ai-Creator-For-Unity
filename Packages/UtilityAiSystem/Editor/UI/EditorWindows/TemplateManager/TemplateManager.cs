using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using UnityEditor.UIElements;
using System.IO;
using System.Threading.Tasks;
using UniRxExtension;
using Unity.Profiling.Editor;

internal class TemplateManager : EditorWindow
{
    private IDisposable activeCollectionChangedSub;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly CompositeDisposable modelsChangedSubscriptions = new CompositeDisposable();

    private VisualElement root;
    private VisualElement leftPanel;
    private VisualElement elementsContainer;
    private VisualElement rightPanel;
    private Button copyButton;
    private Button deleteButton;
    private Button sortButton;
    private PopupField<string> addElementPopup;
    private List<string> dropDownChoices;
    private DropdownField dropDown;
    private TemplateService templateService => TemplateService.Instance;

    private AiObjectViewModel  currentMainWindowViewModel;
    private AiObjectModel selectedModel;
    private PersistenceAPI persistenceAPI => PersistenceAPI.Instance;

    private readonly Dictionary<AiObjectModel, AiObjectViewModel> componentsByModels = new Dictionary<AiObjectModel, AiObjectViewModel>();
    private Toolbar toolbar;
    private long lastClearClick;
    private long clickConfirmationTimeMs = 200;
    
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
        // Load Objects
        root = rootVisualElement;
        DontDestroyOnLoad(this);

        var treeAsset = AssetService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);
        toolbar = root.Q<Toolbar>();

        dropDown = root.Q<DropdownField>("TypeDropdown");

        leftPanel = root.Q<VisualElement>("left-panel");
        elementsContainer = leftPanel.Q<VisualElement>("ButtonContainer");

        root.Q<VisualElement>("Buttons");
        addElementPopup = new PopupField<string>("Add element");
        var addElementPopupContainer = root.Q<VisualElement>("AddElementPopupContainer");
        copyButton = root.Q<Button>("CopyButton");
        deleteButton = root.Q<Button>("DeleteButton");
        sortButton = root.Q<Button>("ClearButton");
        rightPanel = root.Q<VisualElement>("right-panel");

        
        // Init Objects
        InitToolbarFile();
        InitToolbarDebug();
        InitToolBarSettings();
        InitToolbarDemos();
        InitDropdown();
        UpdateLeftPanel();
        addElementPopupContainer.Add(addElementPopup);

        
        // Subscribe
        copyButton.RegisterCallback<MouseUpEvent>(_ =>
        {
            DebugService.Log("TT! Copy Pressed", this);
            CopySelectedElements();
        });

        deleteButton.RegisterCallback<MouseUpEvent>(_ =>
        {
            DeleteSelectedElements();
        });

        sortButton.text = "Sort";
        sortButton.RegisterCallback<MouseUpEvent>(_ =>
        {
            var type = MainWindowService.Instance.GetTypeFromString(dropDown.value);
            var collection = templateService.GetCollection(type) as ReactiveListNameSafe<AiObjectModel>;
            collection?.Sort();
        });
        
        addElementPopup.RegisterCallback<ChangeEvent<string>>(AddAiObject);

        TemplateService.Instance.OnIncludeDemosChanged
            .Subscribe(_ =>
            {
                UpdateAddElementPopup();
            })
            .AddTo(disposables);

        activeCollectionChangedSub = TemplateService.Instance
            .OnCollectionChanged
            .Subscribe(UpdateLeftPanelIfActiveCollectionChanged);

        ProjectSettingsService.Instance.OnProjectSettingsChanged
            .Subscribe(_ =>
            {
                var projectName = ProjectSettingsService.Instance.GetCurrentProjectName();
                titleContent.text = projectName;
            })
            .AddTo(disposables);
    }

    private async void AddAiObject(ChangeEvent<string> evt)
    {
        if (evt.newValue != null && evt.newValue != Consts.LineBreakBaseTypes && evt.newValue != Consts.LineBreakTemplates && evt.newValue != Consts.LineBreakDemos)
        {
            var type = MainWindowService.Instance.GetTypeFromString(dropDown.value);
            var collection = templateService.GetCollection(type);
            var aiObject = await AddCopyService.GetAiObjectClone(evt.newValue, collection.Values);
            templateService.Add(aiObject);
            ModelSelected(aiObject);
        }
        addElementPopup.SetValueWithoutNotify(null);
    }

    void OnEnable()
    {
        // await UasTemplateService.Instance.LoadCurrentProject(true);
        var mws = MainWindowService.Instance;
        // mws.OnUpdateStateChanged
        //     .Subscribe(state =>
        //     {
        //         var projectName = ProjectSettingsService.Instance.GetCurrentProjectName();
        //         this.titleContent.text = state ? projectName + " - Loading" : projectName;
        //     })
        //     .AddTo(disposables);
        mws.Start();

        templateService.OnStateChanged
            .Subscribe(state =>
            {
                var projectName = ProjectSettingsService.Instance.GetCurrentProjectName();
                DebugService.Log("Setting state: " + state, this);
                if (string.IsNullOrEmpty(state))
                {
                    titleContent.text = projectName;
                }
                else
                {
                    titleContent.text = projectName + " - " + state;
                }
            })
            .AddTo(disposables);
    }

    private void InitToolbarFile()
    {
        var menu = new ToolbarMenu
        {
            text = "File"
        };

        menu.menu.AppendAction("New Project", NewProject);

        menu.menu.AppendAction("Save", SaveUas);

        async void SaveProjectAs(DropdownMenuAction _)
        {
            await ProjectSettingsService.Instance.SaveProjectAs();
            rightPanel.Clear();
        }

        menu.menu.AppendAction("Save As", SaveProjectAs);

        menu.menu.AppendAction("Open Project", OpenProject);

 

        menu.menu.AppendAction("Export File(s)", ExportFiles);

        menu.menu.AppendAction("Import File(s)", ImportFiles);

        menu.menu.AppendAction("Reload Project", ReloadProject);

        menu.menu.AppendAction("Save Backup - TEST", _ =>
        {
            Task.Factory.StartNew(() => templateService.Save(true));
        });

        menu.menu.AppendAction("Load Backup - TEST", LoadTemporary);

        menu.menu.AppendAction("Exit", _ =>
        {
            Close();
        });
        
        toolbar.Add(menu);
    }

    private async void LoadTemporary(DropdownMenuAction _)
    {
        await templateService.LoadCurrentProject(true);
    }

    private async void ReloadProject(DropdownMenuAction _)
    {
        await templateService.LoadCurrentProject();
    }

    private void InitToolBarSettings()
    {
        var printDebug = new ToolbarToggle
        {
            text = "Print debug"
        };
        printDebug.SetValueWithoutNotify(DebugService.PrintDebug);
        
        printDebug.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            DebugService.PrintDebug = evt.newValue;
        });
        toolbar.Add(printDebug);
        var reloadPlayAbleAis = new ToolbarButton()
        {
            text = "Force Update Playable Ais"
        };
        reloadPlayAbleAis.RegisterCallback<MouseUpEvent>(_ =>
        {
            templateService.SetState("Saving");
            PlayAbleAiService.Instance.UpdateAisFromTemplateService(true);
            templateService.SetState("Ready");
        });
        toolbar.Add(reloadPlayAbleAis);

        var printSequence = new ToolbarButton()
        {
            text = "Timer Print Sequence"
        };
        printSequence.RegisterCallback<MouseUpEvent>(_ =>
        {
            TimerService.Instance.DebugLogSequence();
        });
        toolbar.Add(printSequence);
    }

    private async void SaveUas(DropdownMenuAction _)
    {
        await templateService.Save();
        await templateService.Save(true);
        // MainThreadDispatcher.StartCoroutine(uASTemplateService.SaveCoroutine());
    }

    private async void ImportFiles(DropdownMenuAction _)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Import",
            ProjectSettingsService.Instance.model.LastProjectDirectory, Consts.FileExtensionsFilters);

        var stateType = FileExtensionService.GetStateTypeFromFileName(path);
        var objectType = FileExtensionService.GetTypeFromFileName(path);
        var loadedMetaData = PersistenceAPI.Instance.Persister.LoadObject(path, stateType);
        var importedObjectState = loadedMetaData.LoadedObject as AiObjectModelSingleFileState;
        var toCollection = templateService.GetCollection(objectType);
        var importedObject =
            await PersistSingleFile.Restore(importedObjectState, objectType) as
                AiObjectModel;
        
        toCollection.Add(importedObject);
    }
    
    private async void ExportFiles(DropdownMenuAction _)
    {
        var saveObjects = new List<AiObjectModel>();
        selectedObjects.ForEach(pair => saveObjects.Add(pair.Key));
        var path = EditorUtility.SaveFolderPanel("Export", ProjectSettingsService.Instance.model.LastProjectDirectory,"Export Folder");
        foreach (var saveObject in saveObjects)
        {
            var savePath = path + "/" + saveObject.Name + "." + FileExtensionService.GetFileExtensionFromType(saveObject.GetType());
            DebugService.Log("Saving to: " + savePath, this);
            await saveObject.SaveToFile(savePath);
        }
        
        // await persistenceAPI.SaveObjectsSingleFilePanelAsync(saveObjects);
    }

    private async void OpenProject(DropdownMenuAction _)
    {
        await PopUpService.AskToSaveIfProjectNotSavedThenSelectProjectToLoad();
    }

    private async void NewProject(DropdownMenuAction _)
    {
        await PopUpService.AskToSaveIfProjectNotSavedThenCreateNew();
        UpdateLeftPanel(dropDown.value);
        rightPanel.Clear();
    }

    private void InitToolbarDebug()
    {
        var menu = new ToolbarMenu
        {
            text = "Debug"
        };

        menu.menu.AppendAction("Timer Print Time Log", _ =>
        {
            TimerService.Instance.DebugLogTime();
        });
        


        menu.menu.AppendAction("Timer Reset", _ =>
        {
            TimerService.Instance.Reset();
        });
        toolbar.Add(menu);
    }

    private void InitToolbarDemos()
    {
        var includeDemos = new ToolbarToggle
        {
            text = "include Demo Files"
        };

        includeDemos.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            TemplateService.Instance.IncludeDemos = evt.newValue;
        });
        includeDemos.SetValueWithoutNotify(TemplateService.Instance.IncludeDemos);

        toolbar.Add(includeDemos);
    }


    private void UpdateButtons()
    {
        copyButton.SetEnabled(SelectedModel != null);
        deleteButton.SetEnabled(SelectedModel != null);
    }

    private void AddNewAiObject(string aiObjectName)
    {
        var aiObject = AssetService.GetInstanceOfType<AiObjectModel>(aiObjectName);
        templateService.Add(aiObject);
        ModelSelected(aiObject);
    }

    private async void CopySelectedElements()
    {
        // var tasks = selectedObjects
        //     .Select(selected => Task.Run(selected.Key.Clone))
        //     .ToList();
        //
        // await Task.WhenAll(tasks);
        // foreach (var task in tasks)
        // {
        //     templateService.Add(task.Result);
        // }
        // if (tasks.Count == 0)
        // {
        //     DebugService.Log("No element selected to copy", this);
        //     return;
        // }
        if(selectedObjects.Count == 0) return;
        AiObjectModel modelToSelect = null;
        var results = new List<AiObjectModel>();
        foreach (var o in selectedObjects)
        {
            var result = o.Key.Clone();
            results.Add(result);
            modelToSelect = result;
        }
        templateService.Add(results);

        SelectedModel = modelToSelect;
    }

    private void DeleteSelectedElements()
    {
        var toDelete = selectedObjects
            .Select(element => element.Key)
            .ToList();
        
        
        foreach(var element in toDelete)
        {
            templateService.Remove(element);
            if (!componentsByModels.ContainsKey(element)) continue;
            var component = componentsByModels[element];
            rightPanel.Remove(component);
            componentsByModels.Remove(element);
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
            if(currentMainWindowViewModel != null)
            {
                currentMainWindowViewModel.style.display = DisplayStyle.None;
            }

            UpdateLeftPanel(evt.newValue);
        });
    }

    private void UpdateLeftPanel(string label = "")
    {
        DebugService.Log("Updating left panel",this);
        titleContent.text = ProjectSettingsService.Instance.GetCurrentProjectName();

        if (string.IsNullOrEmpty(label))
        {
            label = dropDown.value;
        }
        var models = templateService.GetCollection(label);
        UpdateLeftPanelIfActiveCollectionChanged(models);
    }

    private void UpdateLeftPanelIfActiveCollectionChanged(ReactiveList<AiObjectModel> modifiedList)
    {
        var activeCollection = templateService.GetCollection(dropDown.value);
        if (modifiedList != activeCollection)
        {
            DebugService.Log("Skipped updating collections were equal", this);
            return;
        }

        LoadModels(activeCollection.Values);
        UpdateAddElementPopup();
    }

    private void UpdateAddElementPopup()
    {
        var type = MainWindowService.Instance.GetTypeFromString(dropDown.value);
        var collection = templateService.GetCollection(type);
        addElementPopup.choices = AddCopyService.GetChoices(type, collection.Values);
    }

    private void LoadModels(List<AiObjectModel> models)
    {
        SelectedModel = null;
        elementsContainer.Clear();
        buttons.Clear();
        selectedObjects.Clear();
        modelsChangedSubscriptions.Clear();
        DebugService.Log("Loading " + models.Count + " models", this);

        foreach (var model in models)
        {
            DebugService.Log("Loading model: " + model.Name, this);
            var button = new Button
            {
                text = model.GetUiName(),
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft
                }
            };
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                ObjectClicked(model, button, evt);
            });
            elementsContainer.Add(button);
            buttons.Add(button);
            model.OnNameChanged
                .Subscribe(_ => button.text = model.GetUiName())
                .AddTo(modelsChangedSubscriptions);
        }
        UpdateButtons();
    }

    private readonly List<Button> buttons = new List<Button>();
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
                    var m = templateService.GetCollection(dropDown.value).Values[i];
                    var b = buttons[i];
                    selectedObjects.Add(new KeyValuePair<AiObjectModel, Button>(m, b));
                }
            } else if (selectedIndex > highestSelectedIndex)
            {
                selectedObjects.Clear();
                for(var i = highestSelectedIndex; i <= selectedIndex; i++)
                {
                    var m = templateService.GetCollection(dropDown.value).Values[i];
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
    private int selectionCounter;

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
            if (currentMainWindowViewModel != null)
            {
                currentMainWindowViewModel.style.display = DisplayStyle.None;
            }
            if (componentsByModels.ContainsKey(model))
            {
                DebugService.Log("Updating Existing MVC", this);
                currentMainWindowViewModel = componentsByModels[model];
                currentMainWindowViewModel.UpdateUi(model);
                currentMainWindowViewModel.style.display = DisplayStyle.Flex;
            } else
            {
                DebugService.Log("Updating new MVC", this);
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var mvc = MainWindowService.Instance.GetAiObjectComponent(model);
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

                currentMainWindowViewModel = mvc;
                currentMainWindowViewModel.style.display = DisplayStyle.Flex;
            }
            selectionCounter = 0;
        }
    }

    private void OnDisable()
    {
        OnClose();
    }

    private void OnClose()
    {
        WindowOpener.windowPosition = position;
        ClearSubscriptions();
    }

    private void OnDestroy()
    {
        OnClose();
    }

    private void ClearSubscriptions()
    {
        activeCollectionChangedSub?.Dispose();
        modelsChangedSubscriptions.Clear();
        disposables.Clear();
    }
}