﻿
#if CSHARP_7_OR_LATER || (UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6))
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncUpdateTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<AsyncUnit> update;
        AsyncTriggerPromiseDictionary<AsyncUnit> updates;


        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(update, updates);
        }


        void Update()
        {
            TrySetResult(update, updates, AsyncUnit.Default);
        }


        public UniTask UpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref update, ref updates, cancellationToken);
        }


    }
}

#endif

