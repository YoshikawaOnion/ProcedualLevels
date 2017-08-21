﻿using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// モデル側のエントリポイント。
    /// </summary>
    public class EntryFlow
    {
        private IEnumerator Run(IEntryView view)
        {
            var model = new AdventureFlow();

            IAdventureView nextView = null;
            yield return view.GotoAdventureAsync()
                             .Do(x => nextView = x)
                             .ToYieldInstruction();

            var dungeonGen = AssetRepository.I.DungeonGenAsset;
            var gameParameter = AssetRepository.I.GameParameterAsset;
            model.Initialize(dungeonGen, gameParameter, nextView);

            IFlow flow = model;
            while (true)
            {
                yield return flow.Start()
                                 .Do(x => flow = x)
                                 .ToYieldInstruction();
                if (flow == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// ゲームのモデルを実行します。
        /// </summary>
        /// <returns>ゲームの実行が終了すると値の発行されるストリーム。</returns>
        /// <param name="view">エントリポイントのビュー。</param>
        public IObservable<Unit> RunAsync(IEntryView view)
        {
            return Observable.FromCoroutine(() => Run(view));
        }
    }
}