using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

internal class AiViewModel : AiObjectViewModel 
{
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();
    private IDisposable bucketTabSub;
    private readonly TemplateContainer root;
    private readonly DropdownContainerViewModel<UtilityContainerSelector> bucketDropdown;
    private readonly DropdownContainerViewModel<UtilityContainerSelector> decisionDropdown;
    private readonly DropdownDescriptionViewModel<IUtilityScorer> utilityScorerDropdown;
    private Uai uaiModel;
    private readonly VisualElement collectionsContainer;
    private readonly Toggle playableToggle;

    private readonly TabViewViewModel tabView;
    private readonly Button bucketTab;
    private Button settingsTab;
    private readonly HelpBox playAbleHelpBox;

    private readonly CollectionViewModel<Bucket> bucketCollection;

    internal AiViewModel() : base()
    {
        root = AssetService.GetTemplateContainer(GetType().FullName);
        Body.Clear();
        Body.Add(root);
        collectionsContainer = root.Q<VisualElement>("CollectionsContainer");
        playAbleHelpBox = new HelpBox("Not set to playable!", HelpBoxMessageType.Warning);
        tabView = new TabViewViewModel();
        collectionsContainer.Add(tabView);

        bucketCollection = new CollectionViewModel<Bucket>(TemplateService.Instance.Buckets, "Bucket", "Buckets");

        var settingsContainer = new VisualElement();

        playableToggle = new Toggle("Playable")
        {
            name = "Playable-Toggle"
        };

        bucketDropdown = new DropdownContainerViewModel<UtilityContainerSelector>("Bucket Selector");
        settingsContainer.Add(bucketDropdown);
        decisionDropdown = new DropdownContainerViewModel<UtilityContainerSelector>("Decision Selector");
        settingsContainer.Add(decisionDropdown);

        utilityScorerDropdown = new DropdownDescriptionViewModel<IUtilityScorer>();
        settingsContainer.Add(utilityScorerDropdown);
        settingsContainer.name = "SettingsContainer";

        bucketDropdown.name = "DropdownScorerCollection";
        decisionDropdown.name = "DropdownScorerCollection";
        utilityScorerDropdown.name = "DropdownScorerCollection";

        bucketTab = tabView.AddTabGroup("Buckets", bucketCollection);
        settingsTab = tabView.AddTabGroup("Settings", settingsContainer);
         
        playableToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            if (uaiModel == null) return;
            uaiModel.IsPLayAble = evt.newValue;

            UpdateHelpBox(evt.newValue);
        });
    }

    private void UpdateHelpBox(bool hide)
    {
        if (hide)
        {
            playAbleHelpBox.style.display = DisplayStyle.None;
        }
        else
        {
            playAbleHelpBox.style.display = DisplayStyle.Flex;
        }
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        uaiModel = model as Uai;

        ScoreContainer.Add(playableToggle);
        ScoreContainer.Add(playAbleHelpBox);

        Debug.Assert(uaiModel != null, nameof(uaiModel) + " != null");
        bucketTab.text = "Buckets (" + uaiModel.Buckets.Count + ")";
        bucketTabSub?.Dispose();
        bucketTabSub = uaiModel.Buckets.OnValueChanged
            .Subscribe(list => bucketTab.text = "Buckets (" + list.Count + ")");
        
        
        playableToggle.SetValueWithoutNotify(uaiModel.IsPLayAble);
        UpdateHelpBox(playableToggle.value);

        if (uaiModel == null)
        {
            bucketCollection.SetElements(new ReactiveList<Bucket>());
        }
        else
        {
            bucketCollection.SetElements(uaiModel.Buckets);
        }
        subscriptions.Clear();

        var currentDecisionIndex = uaiModel.DecisionSelectors.IndexOf(uaiModel.CurrentDecisionSelector);
        decisionDropdown.UpdateUi(uaiModel.DecisionSelectors, currentDecisionIndex);
        decisionDropdown
            .OnSelectedObjectChanged
            .Subscribe(selector => {
                if (uaiModel != null)
                {
                    uaiModel.CurrentDecisionSelector = selector;
                }
            })
            .AddTo(subscriptions);

        var currentBucketindex = uaiModel.BucketSelectors.IndexOf(uaiModel.CurrentBucketSelector);
        bucketDropdown.UpdateUi(uaiModel.BucketSelectors,currentBucketindex);
        bucketDropdown
            .OnSelectedObjectChanged
            .Subscribe(selector =>
            {
                if (uaiModel != null)
                {
                    uaiModel.CurrentBucketSelector = selector;
                }
            })
            .AddTo(subscriptions);

        utilityScorerDropdown.UpdateUi(ScorerService.Instance.UtilityScorers, "Utility Scorer", uaiModel.UtilityScorer.GetName());
        utilityScorerDropdown
            .OnDropdownValueChanged
            .Subscribe(uS =>
            {
                if (uaiModel != null)
                {
                    uaiModel.UtilityScorer = uS;
                }
            })
            .AddTo(subscriptions);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal AI");
    }

    ~AiViewModel()
    {
        bucketTabSub?.Dispose();
        subscriptions.Clear();
    }
}
