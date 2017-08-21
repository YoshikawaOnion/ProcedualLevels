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

        private IEnumerator TitleFlow(Action<IFlow> callback)
        {
            yield return View.OnTap.Take(1)
                             .ToYieldInstruction();

            IAdventureView nextView = null;
            yield return View.GotoAdventureAsync()
                             .Do(x => nextView = x)
                             .ToYieldInstruction();

            var adventure = new AdventureFlow();
        }

        public IObservable<IFlow> Start()
        {
            IFlow nextFlow = null;
            return Observable.FromCoroutine(() => TitleFlow(x => nextFlow = x))
                             .Select(x => nextFlow)
                             .Take(1);
        }
    }
}