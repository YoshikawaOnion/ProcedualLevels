using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 制限時間を過ぎると敵が現れ始めるスポナーの振る舞いを提供します。
    /// </summary>
    public class TimeLimitSpawnerBehavior : ISpawnerBehavior
    {
        public IObservable<Unit> GetSpawnStream(AdventureContext context)
		{
            var second = Observable.Interval(TimeSpan.FromMinutes(1))
                                   .Merge(Observable.Return((long)0));
			return context.TimeLimit.First(x => x <= 0)
                          .SelectMany(second)
                          .Select(x => Unit.Default);
        }
    }
}