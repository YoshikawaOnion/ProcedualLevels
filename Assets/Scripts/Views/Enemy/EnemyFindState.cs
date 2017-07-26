using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 敵キャラクターがプレイヤーを見つけたかどうかに関する状態の基底クラス。
    /// </summary>
    public abstract class EnemyFindState : ReactiveState<EnemyController>
    {
        public EnemyFindState(EnemyController context) : base(context)
        {
        }

        /// <summary>
        /// プレイヤーを見つけたかどうかの状態を変更させます。
        /// </summary>
        /// <param name="state">新しい状態。</param>
        public void ChangeState(EnemyFindState state)
        {
            Context.FindState = state;
            state.Subscribe();
            Dispose();
        }

        /// <summary>
        /// 移動などの行動が可能なら行動します。
        /// </summary>
        public abstract void Control();
    }
}