using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゲーム本体のエントリポイント。
    /// </summary>
    public class EntryManager : MonoBehaviour, IEntryView
    {
        /// <summary>
        /// 探索画面に遷移します。
        /// </summary>
        /// <returns>繊維が完了すると探索画面のビューを発行するストリーム。</returns>
        public IObservable<IAdventureView> GotoAdventureAsync()
        {
            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            IAdventureView view = Instantiate(viewPrefab);
            return Observable.NextFrame()
                             .Select(x => view)
                             .Take(1);
        }

        /// <summary>
        /// タイトル画面に遷移します。
        /// </summary>
        /// <returns>遷移が完了するとタイトル画面のビューを発行するストリーム。</returns>
        public IObservable<ITitleView> GotoTitleAsync()
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            var model = new Models.EntryFlow();
            model.RunAsync(this)
                 .Subscribe();
        }
    }
}