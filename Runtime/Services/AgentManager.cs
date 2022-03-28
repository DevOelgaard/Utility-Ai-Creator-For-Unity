using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;
using UniRx;
using System;

internal class AgentManager
{
    private static AgentManager instance;
    internal AgentManagerModel Model { get; } = new AgentManagerModel();
    internal List<string> AgentIdentifiers => Model.AgentIdentifiers;

    internal IObservable<bool> AgentTypesUpdated => agentIdentifiersUpdated;
    private Subject<bool> agentIdentifiersUpdated = new Subject<bool>();

    internal IObservable<IAgent> AgentsUpdated => agentsUpdated;
    private Subject<IAgent> agentsUpdated = new Subject<IAgent>();
    internal AgentManager()
    {
        //AiTicker.Instance.Start();
    }

    internal static AgentManager Instance { 
        get 
        { 
            if (instance == null)
            {
                instance = new AgentManager();
            } 
            return instance; 
        } 
    }

    internal void Register(IAgent agent)
    {
        var identifier = agent.TypeIdentifier;
        if (!Model.AgentsByIdentifier.ContainsKey(identifier))
        {
            Model.AgentsByIdentifier.Add(identifier, new ReactiveList<IAgent>());
            AgentIdentifiers.Add(identifier);
            agentIdentifiersUpdated.OnNext(true);

        }

        Model.AgentsByIdentifier[identifier].Add(agent);
        Model.Agents.Add(agent);
        agentsUpdated.OnNext(agent);
    }

    internal void Unregister(IAgent agent)
    {
        var identifier = agent.TypeIdentifier;
        if (Model.AgentsByIdentifier.ContainsKey(identifier))
        {
            Model.AgentsByIdentifier.Remove(identifier);
        }
        Model.Agents.Remove(agent);
        agentsUpdated.OnNext(agent);
    }

    internal ReactiveList<IAgent> GetAgentsByIdentifier(string identifier)
    {
        if (!Model.AgentsByIdentifier.ContainsKey(identifier))
        {
            return new ReactiveList<IAgent>();
        } else
        {
            return Model.AgentsByIdentifier[identifier];
        }
    }
}