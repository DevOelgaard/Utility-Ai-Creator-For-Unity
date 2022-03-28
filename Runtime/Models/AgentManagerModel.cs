using System;
using System.Collections;
using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;

internal class AgentManagerModel
{
    internal Dictionary<string, ReactiveList<IAgent>> AgentsByIdentifier = new Dictionary<string, ReactiveList<IAgent>>();
    internal List<string> AgentIdentifiers = new List<string>();
    internal ReactiveList<IAgent> Agents = new ReactiveList<IAgent>();

}