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
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private CompositeDisposable listViewSubscriptions = new CompositeDisposable();
    private IDisposable collectionUpdatedSub;

    private TemplateContainer root;

    private Button sortCollectionButton;
    private VisualElement tempHeader;
    private PopupField<string> addCopyPopup;

    //private Label titleLabel;
    private ScrollView elementsBody;

    private ReactiveList<T> collection;
    private ReactiveList<AiObjectModel> templates;
    private VisualElement dropdownContainer;

    private List<AiObjectComponent> expandedList = new List<AiObjectComponent>();
    private List<MainWindowFoldedComponent> foldedList = new List<MainWindowFoldedComponent>();

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

        addCopyPopup.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (evt.newValue == null) return;
            AddCopy(evt.newValue);
            addCopyPopup.value = null;
        });

        this.templates = templates;
        this.templates.OnValueChanged
            .Subscribe(_ => InitAddCopyPopup())
            .AddTo(subscriptions);

        InitAddCopyPopup();

        var t = collection.GetType();
        if (t == typeof(ReactiveList<Consideration>))
        {
            sortCollectionButton.text = Consts.Text_Button_SortByPerformance;
            sortCollectionButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                var cast = collection as ReactiveList<Consideration>;
                var sortedList = cast.Values.OrderBy(c => c.PerformanceTag).ToList();
                cast.Clear();
                cast.Add(sortedList);
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
                //sortedList.ForEach(c => cast.Add(c));
            });
        }
        else
        {
            sortCollectionButton.style.display = DisplayStyle.None;
        }
    }

    private void InitAddCopyPopup()
    {
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(typeof(T));
        addCopyPopup.choices = namesFromFiles
            .Where(t => !t.Name.Contains("Mock") && !t.Name.Contains("Stub"))
            .Select(t => t.Name)
            .ToList();


        foreach(var template in templates.Values)
        {
            addCopyPopup.choices.Add(template.Name);
        }
    }

    private void AddCopy(string name)
    {
        T element = templates.Values.FirstOrDefault(t => t.Name == name) as T;

        if (element == null)
        {
            element = AssetDatabaseService.GetInstanceOfType<T>(name);
        }

        element = (T)element.Clone();
        AddElement(element);
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
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        elementsBody.Clear();
        listViewSubscriptions.Clear();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection Init");
        sw.Restart();

        if (collection.Values.Count > expandedList.Count)
        {
            var diff = collection.Values.Count - expandedList.Count;
            var type = collection.Values[0].GetType();
            for(var i = 0; i < diff; i++)
            {
                sw.Restart();
                var expanded = MainWindowService.Instance.RentComponent(type);
                expandedList.Add(expanded);
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection expanded");
                sw.Restart();
                var folded = MainWindowService.Instance.RentMainWindowFoldedComponent();
                TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection folded");
                sw.Restart();
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
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection listView");
            sw.Restart();
            folded.UpdateUi(element);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection  folded.UpdateUi");
            sw.Restart();
            expanded.UpdateUi(element);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection expanded.UpdateUi");
            sw.Restart();
            listView.UpdateUi(expanded, folded);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection listView.UpdateUi");
            sw.Restart();

            listView.OnRemoveClicked
                .Subscribe(_ => collection.Remove(element))
                .AddTo(listViewSubscriptions);

            listView.OnUpClicked
                .Subscribe(_ => collection.DecreaseIndex(element))
                .AddTo(listViewSubscriptions);

            listView.OnDownClicked
                .Subscribe(_ => collection.IncreaIndex(element))
                .AddTo(listViewSubscriptions);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection Subscribe");
            sw.Restart();
            elementsBody.Add(listView);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateCollection Add");
            sw.Restart();
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
    }

    ~CollectionComponent()
    {
        foreach(var comp in expandedList)
        {
            MainWindowService.Instance.ReturnComponent(comp);
        }
         foreach(var comp in foldedList)
        {
            MainWindowService.Instance.ReturnMWFC(comp);
        }
        ClearSubscriptions();
    }
}