using System;
using System.Collections;
using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;

internal class AgentManagerModel
{
    internal readonly Dictionary<string, ReactiveList<IAgent>> AgentsByIdentifier = new Dictionary<string, ReactiveList<IAgent>>();
    internal readonly List<string> AgentIdentifiers = new List<string>();
    internal readonly ReactiveList<IAgent> Agents = new ReactiveList<IAgent>();

}