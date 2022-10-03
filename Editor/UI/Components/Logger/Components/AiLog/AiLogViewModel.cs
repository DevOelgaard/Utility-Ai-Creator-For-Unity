﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class AiLogViewModel : AiObjectLogViewModel
{
    private readonly VisualElement bucketContainer;
    private readonly LogComponentPool<BucketLogViewModel> bucketPool;
    private readonly VisualElement ucsContainer;
    private readonly UCSLogComponent bucketSelctorComponent;
    private readonly UCSLogComponent decisionSelctorComponent;
    
    public AiLogViewModel(): base()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        ucsContainer = new VisualElement();
        ucsContainer.name = "UCSContainer";
        var settingsFoldout = new Foldout();
        settingsFoldout.text = "Settings";
        settingsFoldout.Add(ucsContainer);
        settingsFoldout.value = false;
        root.Add(settingsFoldout);
        bucketContainer = new VisualElement();
        bucketContainer.name = "BucketContainer";
        root.Add(bucketContainer);

        bucketSelctorComponent = new UCSLogComponent("Bucket Selector");
        decisionSelctorComponent = new UCSLogComponent("Decision Selector");
        ucsContainer.Add(bucketSelctorComponent);
        ucsContainer.Add(decisionSelctorComponent);
        bucketPool = new LogComponentPool<BucketLogViewModel>(bucketContainer, true ,"Buckets",1,true,false);
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectDebug)
    {
        var a = (AiLog)aiObjectDebug;

        var logModels = new List<ILogModel>();
        foreach (var b in a.Buckets)
        {
            logModels.Add(b);
        }
        bucketPool.Display(logModels);
        bucketSelctorComponent.UpdateUi(a.BucketSelector);
        decisionSelctorComponent.UpdateUi(a.DecisionSelector);
    }

    internal override void Hide()
    {
        base.Hide();
        bucketPool.Hide();
    }

    internal override void SetColor()
    {
        base.SetColor();

        var list = new List<KeyValuePair<VisualElement,float>>();
        foreach(var b in bucketPool.LogComponents)
        {
            if (b.Model == null) continue;
            var cast = b.Model as BucketLog;
            list.Add(new KeyValuePair<VisualElement, float>(b, cast.Score));
            b.SetColor();
        }
        ColorService.SetColor(list);
    }

    internal override void ResetColor()
    {
        foreach (var b in bucketPool.LogComponents)
        {
            if (b.Model == null) continue;
            var cast = b.Model as BucketLog;
            b.ResetColor();
        }
        base.ResetColor();
    }
}