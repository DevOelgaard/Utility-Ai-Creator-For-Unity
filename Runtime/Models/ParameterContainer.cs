using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ParameterContainer
{
    private readonly Func<List<Parameter>> getParameters;
    private readonly Dictionary<string, Parameter> parametersByName = new Dictionary<string, Parameter>();

    private ParameterContainer()
    {
        
    }
    public ParameterContainer(Func<List<Parameter>> getParameters)
    {
        this.getParameters = getParameters;
        InitializeParameters();
    }

    internal Dictionary<string, Parameter>.ValueCollection Parameters => parametersByName.Values;

    internal void AddParameter(Parameter param)
    {
        if (parametersByName.ContainsKey(param.Name))
        {
            parametersByName[param.Name] = param;
        }
        else
        {
            parametersByName.Add(param.Name, param);
        }
    }

    internal Parameter GetParameter(string parameterName)
    {
        if (!parametersByName.ContainsKey(parameterName))
        {
            var p = Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (p == null)
            {
                DebugService.LogError("Couldn't find parameter: " + parameterName, this);
            }
            parametersByName.Add(parameterName,p);
        }

        return parametersByName[parameterName];
    }


    private void InitializeParameters()
    {
        foreach (var param in getParameters.Invoke())
        {
            AddParameter(param);
        }
    }

    internal ParameterContainer Clone()
    {
        var clone = new ParameterContainer();
        foreach (var parameter in Parameters)
        {
            clone.AddParameter(parameter.Clone());
        }

        return clone;
    }

    public void RestoreFromState(ParameterContainerState state)
    {
        foreach (var parameterState in state.parameterStates)
        {
            var param = GetParameter(parameterState.Name);
            if (param == null)
            {
                DebugService.LogWarning("Parameter with name: " + parameterState.Name + " not found! " +
                                        " If this was a parameter you recently removed from the code don't worry about it.", this);
                continue;
            }

            if (parameterState.OriginalType == typeof(ParameterEnum))
            {
                var s = parameterState as ParameterEnumState;
                var p = param as ParameterEnum;
                p.EnumType = s.EnumType;
                p.Value = Enum.Parse(p.EnumType, s.CurrentEnumSelection);
            }
            else
            {
                param.Value = parameterState.Value;
            }
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
    public List<ParameterState> parameterStates;

    public ParameterContainerState()
    {
    }

    public ParameterContainerState(ParameterContainer parameterContainer)
    {
        parameterStates = new List<ParameterState>();
        if (parameterContainer == null) return;
        foreach (var parameter in parameterContainer.Parameters)
        {
            parameterStates.Add(parameter.GetState() as ParameterState);
        }
    }
}