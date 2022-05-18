using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;
using UniRx;
using System;

internal class AgentManager
{
    private static AgentManager _instance;
    internal AgentManagerModel Model { get; } = new AgentManagerModel();
    internal List<string> AgentIdentifiers => Model.AgentIdentifiers;

    internal IObservable<bool> AgentIdentifiersUpdated => agentIdentifiersUpdated;
    private readonly Subject<bool> agentIdentifiersUpdated = new Subject<bool>();

    internal IObservable<IAgent> AgentsUpdated => agentsUpdated;
    private readonly Subject<IAgent> agentsUpdated = new Subject<IAgent>();
    private AgentManager()
    {
        //AiTicker.Instance.Start();
    }

    internal static AgentManager Instance { 
        get { return _instance ??= new AgentManager(); } 
    }

    internal void Reset()
    {
        _instance = new AgentManager();
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