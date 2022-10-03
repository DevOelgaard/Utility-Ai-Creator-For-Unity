﻿// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// public class RestoreAbleCollection: RestoreAble
// {
//     internal List<RestoreAble> Models = new List<RestoreAble>();
//     public Type Type { get; private set; }
//     public RestoreAbleCollection()
//     {
//     }
//
//     public RestoreAbleCollection(List<RestoreAble> states, Type type)
//     {
//         this.Models = states;
//         this.Type = type;
//     }
//
//     internal override RestoreState GetState()
//     {
//         return new RestoreAbleCollectionState(Models, Type, this);
//     }
//
//     protected override string GetFileName()
//     {
//         return "RestoreableCollection";
//     }
//
//
//     protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
//     {
//         var s = state as RestoreAbleCollectionState;
//         Models = new List<RestoreAble>();
//         s.States.ForEach(e =>
//         {
//             Models.Add(await Restore<RestoreAble>(e, restoreDebug));
//         });
//         Type = Type.GetType(s.DerivedTypeString);
//     }
//     private async Task<
//
//     protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
//     {
//         persister.SaveObject(state, path);
//     }
// }
//
// public class RestoreAbleCollectionState: RestoreState
// {
//     public readonly List<RestoreState> States;
//     // public string TypeString;
//
//     public RestoreAbleCollectionState()
//     {
//     }
//
//     internal RestoreAbleCollectionState(List<RestoreAble> restoreAbles, Type typeString, RestoreAbleCollection o) : base(o)
//     {
//         States = new List<RestoreState>();
//         restoreAbles.ForEach(e =>
//         {
//             States.Add(e.GetState());
//         });
//         DerivedTypeString = typeString.ToString();
//     }
// }
