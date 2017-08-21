using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface IEntryView
    {
        /// <summary>
        /// タイトル画面に遷移します。
        /// </summary>
        /// <returns>遷移が完了するとタイトル画面のビューを発行するストリーム。</returns>
        IObservable<ITitleView> GotoTitleAsync();

        /// <summary>
        /// 探索画面に遷移します。
        /// </summary>
        /// <returns>繊維が完了すると探索画面のビューを発行するストリーム。</returns>
        IObservable<IAdventureView> GotoAdventureAsync();
    }
}