using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class ResultFlow : IFlow
    {
        private int RestTime { get; set; }
        private int Score { get; set; }
        public IResultView View { get; set; }

        public ResultFlow(int restTime, int score, IResultView view)
        {
            View = view;
            Score = score;
            RestTime = restTime;
        }

        public IObservable<IFlow> Start()
        {
            return Observable.FromCoroutine<IFlow>(observer => RunAsync(observer));
        }

        private IEnumerator RunAsync(IObserver<IFlow> result)
        {
            yield return Observable.Timer(TimeSpan.FromSeconds(0.8))
                                   .ToYieldInstruction();

            var finalScore = Score + (int)(RestTime * AssetRepository.I.GameParameterAsset.ScoreParSecond);

            // アニメーションする。タップされたら飛ばす
            yield return View.AnimateBonusAsync(RestTime, Score, finalScore)
                             .TakeUntil(View.OnTap)
                             .ToYieldInstruction();

            View.ResetScoreBoard(0, finalScore);

            yield return Observable.Timer(TimeSpan.FromSeconds(0.3))
                                   .ToYieldInstruction();

            View.ShowTapInstruction();

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