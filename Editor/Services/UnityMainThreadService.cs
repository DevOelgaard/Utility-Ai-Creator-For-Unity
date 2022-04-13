using System;
using System.Collections;
using System.Collections.Concurrent;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

// TODO Delete this
[InitializeOnLoad]
internal class UnityMainThreadService
{
        private static readonly ConcurrentQueue<Action> MainThreadActions = new ConcurrentQueue<Action>();
        private Coroutine mainThreadCoroutine;
        private static readonly UnityMainThreadService intance;

        static UnityMainThreadService()
        {
                intance = new UnityMainThreadService();
                EditorCoroutineUtility.StartCoroutine(InvokeActions(), intance);
        }

        private static Action _currentAction;

        private static IEnumerator InvokeActions()
        {
                while (true)
                {
                        if (!MainThreadActions.IsEmpty && _currentAction == null)
                        {
                                MainThreadActions.TryDequeue(out _currentAction);
                        }

                        if (_currentAction != null)
                        {
                                _currentAction.Invoke();
                                _currentAction = null;
                        }
                        yield return null;
                }

                yield return null;
        }

        internal static void InvokeActionOnMainThread(Action action)
        {
                MainThreadActions.Enqueue(action);
        }
}