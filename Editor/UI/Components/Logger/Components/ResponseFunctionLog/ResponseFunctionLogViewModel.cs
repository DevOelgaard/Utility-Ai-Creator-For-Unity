﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ResponseFunctionLogViewModel: LogComponent
{
    private readonly Label typeLabel;
    private readonly VisualElement body;
    private ResponseFunctionLog rf;
    //private ParameterLogComponentPool pool;
    private readonly LogComponentPool<ParameterLogViewModel> parameterPool;
    public ResponseFunctionLogViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        typeLabel = root.Q<Label>("Type-Label");
        body = root.Q<VisualElement>("Body");
        parameterPool = new LogComponentPool<ParameterLogViewModel>(body, false,"Parameters", 2);
    }

    internal override string GetUiName()
    {
        return rf.Type;
    }

    internal override void UpdateUi(ILogModel element)
    {
        rf = element as ResponseFunctionLog;
        typeLabel.text = rf.Type.ToString();
        var logModels = new List<ILogModel>();
        foreach(var p in rf.Parameters)
        {
            logModels.Add(p);
        }
        parameterPool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        parameterPool.Hide();
    }
}