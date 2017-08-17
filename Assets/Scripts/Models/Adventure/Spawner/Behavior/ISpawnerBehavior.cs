using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// スポナーの振る舞いを提供するインターフェース。
    /// </summary>
    public interface ISpawnerBehavior
    {
        /// <summary>
        /// スポナーから敵が現れるタイミングを制御するストリームを取得します。
        /// </summary>
        /// <returns>敵が現れるタイミングを制御するストリーム。</returns>
        /// <param name="context">探索シーンのコンテキスト クラス。</param>
        IObservable<Unit> GetSpawnStream(AdventureContext context);
    }
}