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
        /// <summary>
        /// スポナーから敵が現れるタイミングを制御するストリームを取得します。
        /// </summary>
        /// <returns>敵が現れるタイミングを制御するストリーム。</returns>
        /// <param name="context">探索シーンのコンテキスト クラス。</param>
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