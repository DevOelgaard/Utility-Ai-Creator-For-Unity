using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using UniRx;
using UniRxExtension;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectionComponent<T> : VisualElement where T : AiObjectModel
{
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();
    private readonly CompositeDisposable listViewSubscriptions = new CompositeDisposable();
    private readonly CompositeDisposable nameChangeSubscriptions = new CompositeDisposable();
    private IDisposable collectionUpdatedSub;

    private readonly TemplateContainer root;

    private readonly Button sortCollectionButton;
    private VisualElement tempHeader;
    private readonly PopupField<string> addCopyPopup;

    //private Label titleLabel;
    private readonly ScrollView elementsBody;

    private ReactiveList<T> collection;
    private readonly ReactiveList<AiObjectModel> templates;
    private readonly VisualElement dropdownContainer;

    private readonly List<AiObjectComponent> expandedList = new List<AiObjectComponent>();
    private readonly List<MainWindowFoldedComponent> foldedList = new List<MainWindowFoldedComponent>();

    public IObservable<bool> OnSortClicked => onSortClicked;
    private readonly Subject<bool> onSortClicked = new Subject<bool>();

    public CollectionComponent(ReactiveList<AiObjectModel> templates, string tempLabel, string elementsLabel, string dropDownLabel = "Templates")
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType());
        Add(root);
        root.styleSheets.Add(StylesService.GetStyleSheet("CollectionComponent"));

        this.collection = new ReactiveList<T>();
        this.templates = templates;

        sortCollectionButton = root.Q<Button>("SortCollection-Button");
        elementsBody = root.Q<ScrollView>("ElementsBody");
        tempHeader = root.Q<VisualElement>("TempHeader");
        dropdownContainer = root.Q<VisualElement>("DropdownContainer");
        addCopyPopup = new PopupField<string>("Add " + tempLabel);
        dropdownContainer.Add(addCopyPopup);

        addCopyPopup.RegisterCallback<ChangeEvent<string>>(OnAddCopyValueChanged);

        this.templates = templates;
        this.templates.OnValueChanged
            .Subscribe(temps =>
            {
                InitAddCopyPopup();
                nameChangeSubscriptions.Clear();
                foreach(var t in temps)
                {
                    t
                    .OnNameChanged
                    .Subscribe(t =>
                    {
                        InitAddCopyPopup();
                    })
                    .AddTo(nameChangeSubscriptions);
                }
            })
            .AddTo(subscriptions);

        InitAddCopyPopup();

        var t = collection.GetType();
        if (t == typeof(ReactiveList<Consideration>))
        {
            sortCollectionButton.text = Consts.Text_Button_SortByPerformance;
            sortCollectionButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                onSortClicked.OnNext(true);
            });
        }
        else if (t == typeof(ReactiveList<Bucket>))
        {
            sortCollectionButton.text = Consts.Text_Button_SortByWeight;
            sortCollectionButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                var cast = collection as ReactiveList<Bucket>;
                var sortedList = cast.Values.OrderByDescending(b => b.Weight.Value).ToList();
                cast.Clear();
                cast.Add(sortedList);
            });
        }
        else
        {
            sortCollectionButton.style.display = DisplayStyle.None;
        }

        UasTemplateService.Instance.OnIncludeDemosChanged
            .Subscribe(value =>
            {
                InitAddCopyPopup();
            })
            .AddTo(subscriptions);
    }

    private async void OnAddCopyValueChanged(ChangeEvent<string> evt)
    {
        if (evt.newValue == null) return;
        await AddCopy(evt.newValue);
        addCopyPopup.value = null;
    }

    private void InitAddCopyPopup()
    {
        addCopyPopup.choices = AddCopyService.GetChoices(typeof(T));

        foreach(var template in templates.Values)
        {
            addCopyPopup.choices.Add(template.Name);
        }
    }

    private async Task AddCopy(string aiObjectName)
    {
        var whiteSpaceName = StringService.SpaceBetweenUpperCase(aiObjectName);
        var noWhiteSpace = StringService.RemoveWhiteSpaces(aiObjectName);
        var existingElement =
            templates.Values.FirstOrDefault(t =>
                t.Name == aiObjectName || t.Name == whiteSpaceName || t.Name == noWhiteSpace); 
        
        if(existingElement != null)
        {
            var c = await existingElement.CloneAsync();
            AddElement(c as T);

        } else
        {
            var element = AssetDatabaseService.GetInstanceOfType<T>(noWhiteSpace);
            AddElement(element);
        }
    }

    internal void AddElement(T element)
    {
        collection.Add(element);
    }

    internal void SetElements(ReactiveList<T> elements)
    {

        this.collection = elements;
        collectionUpdatedSub?.Dispose();
        collectionUpdatedSub = collection.OnValueChanged
            .Subscribe(_ => UpdateCollection());
        UpdateCollection();
    }

    private void UpdateCollection()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        elementsBody.Clear();
        listViewSubscriptions.Clear();
        if (collection.Values.Count > expandedList.Count)
        {
            var diff = collection.Values.Count - expandedList.Count;
            var type = collection.Values[0].GetType();
            for(var i = 0; i < diff; i++)
            {
                var expanded = MainWindowService.Instance.GetAiObjectComponent(type);
                expandedList.Add(expanded);
                var folded = MainWindowService.Instance.GetMainWindowFoldedComponent();
                foldedList.Add(folded);
            }
        }
        for (var i = 0; i < collection.Values.Count; i++)
        {
            var element = collection.Values[i];
            var folded = foldedList[i];
            folded.style.display = DisplayStyle.Flex;

            var expanded = expandedList[i];
            expanded.style.display = DisplayStyle.Flex;

            var listView = new ListViewComponent();
            folded.UpdateUi(element);
            expanded.UpdateUi(element);
            listView.UpdateUi(expanded, folded);

            listView.OnRemoveClicked
                .Subscribe(_ => collection.Remove(element))
                .AddTo(listViewSubscriptions);

            listView.OnUpClicked
                .Subscribe(_ => collection.DecreaseIndex(element))
                .AddTo(listViewSubscriptions);

            listView.OnDownClicked
                .Subscribe(_ => collection.IncreaIndex(element))
                .AddTo(listViewSubscriptions);
            elementsBody.Add(listView);
        }

        if(expandedList.Count > collection.Count)
        {
            for(var i = collection.Count; i < expandedList.Count; i++)
            {
                expandedList[i].style.display = DisplayStyle.None;
                foldedList[i].style.display = DisplayStyle.None;
            }
        }
    }

    private void ClearSubscriptions()
    {
        collectionUpdatedSub?.Dispose();
        listViewSubscriptions.Clear();
        subscriptions.Clear();
        nameChangeSubscriptions?.Clear();
    }

    ~CollectionComponent()
    {
        // foreach(var comp in expandedList)
        // {
        //     MainWindowService.Instance.ReturnComponent(comp);
        // }
        //  foreach(var comp in foldedList)
        // {
        //     MainWindowService.Instance.ReturnMWFC(comp);
        // }
        ClearSubscriptions();
    }
}