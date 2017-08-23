using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    public class TitleManager : IFlow
    {
        public ITitleView View { get; set; }

        public TitleManager(ITitleView view)
        {
            View = view;
        }

        private IEnumerator RunAsync(IObserver<IFlow> result)
        {
            yield return View.OnTap.Take(1)
                             .ToYieldInstruction();

            IAdventureView nextView = null;
            yield return View.GotoAdventureAsync()
                             .Do(x => nextView = x)
                             .ToYieldInstruction();

            var adventure = new AdventureFlow();
            adventure.Initialize(AssetRepository.I.DungeonGenAsset,
                                 AssetRepository.I.GameParameterAsset,
                                 nextView);
            result.OnNext(adventure);
            result.OnCompleted();
        }

        public IObservable<IFlow> Start()
        {
            return Observable.FromCoroutine<IFlow>(observer => RunAsync(observer));
        }
    }
}