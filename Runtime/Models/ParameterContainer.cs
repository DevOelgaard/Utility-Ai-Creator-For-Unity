using System;
using System.Collections.Generic;
using System.Linq;

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
}