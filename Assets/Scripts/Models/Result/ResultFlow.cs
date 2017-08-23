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
            yield return Observable.Timer(TimeSpan.FromSeconds(1));

            var finalScore = (int)(Score * AssetRepository.I.GameParameterAsset.ScoreParSecond);

            yield return View.AnimateBonusAsync(RestTime, Score, finalScore)
                             .TakeUntil(View.OnTap)
                             .ToYieldInstruction();

            View.ResetScoreBoard(0, finalScore);

            yield return Observable.Timer(TimeSpan.FromSeconds(1));

            result.OnNext(null);
            result.OnCompleted();
        }
    }
}