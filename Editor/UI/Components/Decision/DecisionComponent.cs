using UnityEngine.UIElements;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;

internal class DecisionComponent : AiObjectComponent 
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly TemplateContainer root;
    private readonly VisualElement parametersContainer;

    private readonly CollectionComponent<Consideration> considerationCollections;
    private readonly CollectionComponent<AgentAction> agentActionCollection;
    private Decision decision;

    private readonly TabViewComponent tabView;
    private readonly Button considerationsTab;
    private readonly Button actionsTab;
    internal DecisionComponent(): base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("Parameters");

        considerationCollections = new CollectionComponent<Consideration>(TemplateService.Instance.Considerations, "Consideration", "Considerations");
        agentActionCollection = new CollectionComponent<AgentAction>(TemplateService.Instance.AgentActions, "Action", "Actions");

        tabView = new TabViewComponent();
        root.Add(tabView);
        considerationsTab = tabView.AddTabGroup("Considerations", considerationCollections);
        actionsTab = tabView.AddTabGroup("Actions", agentActionCollection);
        Body.Clear();
        Body.Add(root);
    }
    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        disposables.Clear();
        decision = model as Decision;

        considerationsTab.text = "Considerations (" + decision.Considerations.Count + ")";
        actionsTab.text = "Actions (" + decision.AgentActions.Count + ")";

        decision.Considerations.OnValueChanged
            .Subscribe(list => considerationsTab.text = "Considerations (" + list.Count + ")")
            .AddTo(disposables);

        decision.AgentActions.OnValueChanged
            .Subscribe(list => actionsTab.text = "Actions (" + list.Count + ")")
            .AddTo(disposables);

        considerationCollections.SetElements(decision.Considerations);
        considerationCollections
            .OnSortClicked
            .Subscribe(_ => decision.SortConsiderations())
            .AddTo(disposables);
        agentActionCollection.SetElements(decision.AgentActions);
        SetParameters();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal Decision");

    }

    private void SetParameters()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        parametersContainer.Clear();

        foreach (var parameter in decision.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }



    ~DecisionComponent()
    {
        disposables.Clear();
    }
}