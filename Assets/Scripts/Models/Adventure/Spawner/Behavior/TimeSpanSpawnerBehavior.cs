using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 一定時間ごとに敵が現れるスポナーの振る舞いを提供します。
    /// </summary>
    public class TimeSpanSpawnerBehavior : ISpawnerBehavior
    {
        private TimeSpan TimeSpan;

        /// <summary>
        /// パラメーターを指定して、TimeSpanSpawnerBehaviorの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="timeSpan">敵が現れる時間間隔。</param>
        public TimeSpanSpawnerBehavior(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }

        /// <summary>
        /// スポナーから敵が現れるタイミングを制御するストリームを取得します。
        /// </summary>
        /// <returns>敵が現れるタイミングを制御するストリーム。</returns>
        /// <param name="context">探索シーンのコンテキスト クラス。</param>
        public IObservable<Unit> GetSpawnStream(AdventureContext context)
        {
            return Observable.Interval(TimeSpan)
                             .Select(x => Unit.Default);
        }
    }
}