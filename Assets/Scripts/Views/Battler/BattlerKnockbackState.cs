using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// キャラクターのノックバックに関わる状態の基底クラス。
    /// </summary>
    public abstract class BattlerKnockbackState : ReactiveState<BattlerController>
	{

        public BattlerKnockbackState(BattlerController context) : base(context)
        {
        }

        /// <summary>
        /// この状態の処理を開始します。
        /// </summary>
        public override void Subscribe()
        {
            Context.UpdateAsObservable()
                   .Subscribe(x => Control())
                   .AddTo(Disposable);
        }

        /// <summary>
        /// ノックバック状態を変更します。
        /// </summary>
        /// <param name="state">新しいノックバック状態。</param>
        public void ChangeState(BattlerKnockbackState state)
        {
            if (state.GetType() == GetType())
            {
                Debug.LogWarning("Transit to same state!");
            }
            Context.KnockbackState = state;
            state.Subscribe();
            Dispose();
        }

        /// <summary>
        /// 移動などの行動ができれば行動します。
        /// </summary>
        protected abstract void Control();

        /// <summary>
        /// ノックバックができる状態であればノックバックします。
        /// </summary>
        /// <param name="against">ノックバックを起こした相手のビュー。</param>
        /// <param name="power">ノックバックの強さ。</param>
        public abstract void Knockback(BattlerController against, int power);
    }
}