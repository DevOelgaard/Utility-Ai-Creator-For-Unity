using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ParameterContainer
{
    private readonly Dictionary<string, ParamBool> paramBoolByName = new Dictionary<string, ParamBool>();
    private readonly Dictionary<string, ParamColor> paramColorByName = new Dictionary<string, ParamColor>();
    private readonly Dictionary<string, ParamEnum> paramEnumByName = new Dictionary<string, ParamEnum>();
    private readonly Dictionary<string, ParamFloat> paramFloatByName = new Dictionary<string, ParamFloat>();
    private readonly Dictionary<string, ParamInt> paramIntByName = new Dictionary<string, ParamInt>();
    private readonly Dictionary<string, ParamString> paramStringByName = new Dictionary<string, ParamString>();
    
    public ParameterContainer()
    {
        
    }

    // TODO Init this
    internal List<ParamBase> Parameters => ReloadParameters();

    private List<ParamBase> ReloadParameters()
    {
        // Improve performance of this
        var parameters = BoolParameters.Cast<ParamBase>().ToList();
        parameters.AddRange(ColorParameters.Cast<ParamBase>().ToList());
        parameters.AddRange(EnumParameters.Cast<ParamBase>().ToList());
        parameters.AddRange(FloatParameters.Cast<ParamBase>().ToList());
        parameters.AddRange(IntParameters.Cast<ParamBase>().ToList());
        parameters.AddRange(StringParameters.Cast<ParamBase>().ToList());
        return parameters;
    }

    // internal Dictionary<string, Parameter>.ValueCollection Parameters => parametersByName.Values;
    internal Dictionary<string, ParamBool>.ValueCollection BoolParameters => paramBoolByName.Values;
    internal Dictionary<string, ParamColor>.ValueCollection ColorParameters => paramColorByName.Values;
    internal Dictionary<string, ParamEnum>.ValueCollection EnumParameters => paramEnumByName.Values;
    internal Dictionary<string, ParamFloat>.ValueCollection FloatParameters => paramFloatByName.Values;
    internal Dictionary<string, ParamInt>.ValueCollection IntParameters => paramIntByName.Values;
    internal Dictionary<string, ParamString>.ValueCollection StringParameters => paramStringByName.Values;

    public void AddParameter(string parameterName, object value)
    {
        var type = value.GetType();
        
        if (type == typeof(bool))
        {
            var param = new ParamBool(parameterName, (bool) value);
            if (paramBoolByName.ContainsKey(parameterName))
            {
                paramBoolByName[parameterName] = param;
            }
            else
            {
                paramBoolByName.Add(parameterName,param);
            }
        } 
        else if (type == typeof(Color))
        {
            var param = new ParamColor(parameterName, (Color) value);
            if (paramColorByName.ContainsKey(parameterName))
            {
                paramColorByName[parameterName] = param;
            }
            else
            {
                paramColorByName.Add(parameterName,param);
            }
        }
        else if (type == typeof(Enum) || type.IsSubclassOf(typeof(Enum)))
        {
            var param = new ParamEnum(parameterName, (Enum) value);
            if (paramEnumByName.ContainsKey(parameterName))
            {
                paramEnumByName[parameterName] = param;
            }
            else
            {
                paramEnumByName.Add(parameterName,param);
            }
        } 
        else if (type == typeof(float))
        {
            var param = new ParamFloat(parameterName, (float) value);
            if (paramFloatByName.ContainsKey(parameterName))
            {
                paramFloatByName[parameterName] = param;
            }
            else
            {
                paramFloatByName.Add(parameterName,param);
            }
        } 
        else if (type == typeof(int))
        {
            var param = new ParamInt(parameterName, (int) value);
            if (paramIntByName.ContainsKey(parameterName))
            {
                paramIntByName[parameterName] = param;
            }
            else
            {
                paramIntByName.Add(parameterName,param);
            }
        } 
        else if (type == typeof(string))
        {
            var param = new ParamString(parameterName, (string) value);
            if (paramStringByName.ContainsKey(parameterName))
            {
                paramStringByName[parameterName] = param;
            }
            else
            {
                paramStringByName.Add(parameterName,param);
            }
        } 
        // Catch unsupported parameter types
        else
        {
            throw new ArgumentException("The chosen parameter type isn't supported");
        }
    }

    public ParamBool GetParamBool(string parameterName)
    {
        try
        {
            return paramBoolByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }
    public ParamColor GetParamColor(string parameterName)
    {
        try
        {
            return paramColorByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }
    public ParamEnum GetParamEnum(string parameterName)
    {
        try
        {
            return paramEnumByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }
    public ParamFloat GetParamFloat(string parameterName)
    {
        try
        {
            return paramFloatByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }
    public ParamInt GetParamInt(string parameterName)
    {
        try
        {
            return paramIntByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }
    public ParamString GetParamString(string parameterName)
    {
        try
        {
            return paramStringByName[parameterName];
        }
        catch (Exception e)
        {
            DebugService.LogError(Consts.ErrorMsg_ParameterNotFound,this,e);
            throw;
        }
    }

    internal ParameterContainer Clone()
    {
        var clone = new ParameterContainer();
        foreach (var parameter in paramBoolByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamBool;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }
        foreach (var parameter in paramColorByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamColor;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }
        foreach (var parameter in paramEnumByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamEnum;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }
        foreach (var parameter in paramFloatByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamFloat;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }
        foreach (var parameter in paramIntByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamInt;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }
        foreach (var parameter in paramStringByName)
        {
            var clonedParam = parameter.Value.Clone() as ParamString;
            clone.AddParameter(clonedParam.Name,clonedParam.Value);
        }

        return clone;
    }

    public void RestoreFromState(ParameterContainerState state)
    {
        foreach (var parameterState in state.paramBoolStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
        foreach (var parameterState in state.paramColorStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
        foreach (var parameterState in state.paramEnumStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
        foreach (var parameterState in state.paramFloatStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
        foreach (var parameterState in state.paramIntStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
        foreach (var parameterState in state.paramStringStates)
        {
            AddParameter(parameterState.Name,parameterState.Value);
        }
    }

    internal ParameterContainerState GetState()
    {
        return new ParameterContainerState(this);
    }
}

[Serializable]
public class ParameterContainerState
{
    public List<ParamBaseState<bool>> paramBoolStates;
    public List<ParamColorState> paramColorStates;
    public List<ParamEnumState> paramEnumStates;
    public List<ParamBaseState<float>> paramFloatStates;
    public List<ParamBaseState<int>> paramIntStates;
    public List<ParamBaseState<string>> paramStringStates;
    

    public ParameterContainerState()
    {
    }

    public ParameterContainerState(ParameterContainer parameterContainer)
    {
        paramBoolStates = new List<ParamBaseState<bool>>();
        paramColorStates = new List<ParamColorState>();
        paramEnumStates = new List<ParamEnumState>();
        paramFloatStates = new List<ParamBaseState<float>>();
        paramIntStates = new List<ParamBaseState<int>>();
        paramStringStates = new List<ParamBaseState<string>>();
        if (parameterContainer == null) return;
        
        foreach (var parameter in parameterContainer.BoolParameters)
        {
            paramBoolStates.Add(parameter.GetState() as ParamBaseState<bool>);
        }
        foreach (var parameter in parameterContainer.ColorParameters)
        {
            paramColorStates.Add(parameter.GetState() as ParamColorState);
        }
        foreach (var parameter in parameterContainer.EnumParameters)
        {
            paramEnumStates.Add(parameter.GetState() as ParamEnumState);
        }
        foreach (var parameter in parameterContainer.FloatParameters)
        {
            paramFloatStates.Add(parameter.GetState() as ParamBaseState<float>);
        }
        foreach (var parameter in parameterContainer.IntParameters)
        {
            paramIntStates.Add(parameter.GetState() as ParamBaseState<int>);
        }
        foreach (var parameter in parameterContainer.StringParameters)
        {
            paramStringStates.Add(parameter.GetState() as ParamBaseState<string>);
        }
    }
}