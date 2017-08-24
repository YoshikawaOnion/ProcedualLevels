using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class GameOverFlow : IFlow
    {
        private IGameOverView View { get; set; }

        public GameOverFlow(IGameOverView view)
        {
            View = view;
        }

        public IObservable<IFlow> Start()
        {
            return Observable.FromCoroutine<IFlow>(observer => RunAsync(observer));
        }

        private IEnumerator RunAsync(IObserver<IFlow> result)
        {
            yield return View.OnTap.Take(1).ToYieldInstruction();

            IAdventureView nextView = null;
            yield return View.GotoAdventure()
                             .Do(x => nextView = x)
                             .ToYieldInstruction();

            var nextFlow = new AdventureFlow();
            nextFlow.Initialize(nextView);
            result.OnNext(nextFlow);
            result.OnCompleted();
        }
    }
}