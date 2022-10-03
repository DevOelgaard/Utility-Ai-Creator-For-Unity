using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class BucketLogViewModel : AiObjectLogViewModel
{
    private readonly VisualElement considerationsContainer;
    private readonly VisualElement decisionsContainer;
    private readonly LogComponentPool<ConsiderationLogViewModel> considerationsPool;
    private readonly LogComponentPool<DecisionLogViewModel> decisionsPool;
    private readonly ScoreLogComponent weight;
    private readonly ScoreLogComponent score;
    private BucketLog bucketLog;

    public BucketLogViewModel() : base()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        considerationsContainer = root.Q<VisualElement>("ConsiderationsContainer");
        decisionsContainer = root.Q<VisualElement>("DecisionsContainer");

        considerationsPool = new LogComponentPool<ConsiderationLogViewModel>(considerationsContainer, true,"Considerations", 3, false);
        decisionsPool = new LogComponentPool<DecisionLogViewModel>(decisionsContainer, true,"Decisions", 2, false);

        weight = new ScoreLogComponent("Weight", 0.ToString());
        ScoreContainer.Add(weight);

        score = new ScoreLogComponent("Score", 0.ToString());
        ScoreContainer.Add(score);
    }

    internal override string GetUiName()
    {
        var name = base.GetUiName() + " W: " + bucketLog.Weight.ToString("0.00") + " S: " + bucketLog.Score.ToString("0.00");
        if (IsSelected)
        {
            name += " *S*";
        }
        else if (!IsEvaluated)
        {
            name += " *!E*";
        }

        return name;
    }

    protected override void UpdateUiInternal(AiObjectLog aiLog)
    {
        bucketLog = aiLog as BucketLog;

        score.UpdateScore(bucketLog.Score);

        var logModels = new List<ILogModel>();
        foreach (var c in bucketLog.Considerations)
        {
            logModels.Add(c);
        }
        considerationsPool.Display(logModels);

        logModels.Clear();
        foreach (var d in bucketLog.Decisions)
        {
            logModels.Add(d);
        }
        decisionsPool.Display(logModels);

        weight.UpdateScore(bucketLog.Weight);
    }

    internal override void Hide()
    {
        base.Hide();
        considerationsPool.Hide();
        decisionsPool.Hide();
    }

    internal override void SetColor()
    {
        base.SetColor();

        var list = new List<KeyValuePair<VisualElement, float>>();
        foreach (var c in considerationsPool.LogComponents)
        {
            if (c.Model == null) continue;
            var cast = c.Model as ConsiderationLog;
            list.Add(new KeyValuePair<VisualElement, float>(c, cast.NormalizedScore));
        }
        ColorService.SetColor(list);

        list = new List<KeyValuePair<VisualElement, float>>();
        foreach (var d in decisionsPool.LogComponents)
        {
            if (d.Model == null) continue;
            var cast = d.Model as DecisionLog;
            list.Add(new KeyValuePair<VisualElement, float>(d, cast.Score));
            d.SetColor();
        }
        ColorService.SetColor(list);
    }

    internal override void ResetColor()
    {
        foreach (var c in considerationsPool.LogComponents)
        {
            if (c.Model == null) continue;
            var cast = c.Model as ConsiderationLog;
            c.ResetColor();
        }

        foreach (var d in decisionsPool.LogComponents)
        {
            if (d.Model == null) continue;
            var cast = d.Model as DecisionLog;
            d.ResetColor();
        }
        base.ResetColor();
    }
}
