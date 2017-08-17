using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
	/// <summary>
	/// ノックバックをしていない状態の機能を提供します。
	/// </summary>
	public class BattlerKnockbackStateNeutral : BattlerKnockbackState
	{
        public BattlerKnockbackStateNeutral(BattlerController context) : base(context)
        {
        }

        /// <summary>
        /// 移動などの行動ができれば行動します。
        /// </summary>
        protected override void Control()
        {
            Context.Control();
        }

        /// <summary>
        /// ノックバックができる状態であればノックバックします。
        /// </summary>
        /// <param name="against">ノックバックを起こした相手のビュー。</param>
        /// <param name="power">ノックバックの強さ。</param>
        public override void Knockback(KnockbackInfo info, BattlerController against)
        {
            var direction = -(against.transform.position - Context.transform.position)
                .MergeY(0)
                .normalized;
            var force = direction * info.KnockbackPower;
            var jump = new Vector3(0, info.KnockbackJumpPower, 0);

            Context.Rigidbody.velocity = Vector3.zero;
            Context.Rigidbody.AddForce(force + jump);

            ChangeState(new BattlerKnockbackStateKnockback(Context, against, info));

            var jumpState = Context.GetComponent<HeroMoveController>();
            if (jumpState != null)
            {
                jumpState.SetJumpState();
            }
        }

        /// <summary>
        /// 派生クラスで実装されることで、キャラクターがプレイヤーに踏みつけられた時の処理を実行します。
        /// </summary>
        public override void OnTrampled()
        {
            ChangeState(new BattlerKnockbackStateTrampled(Context));
        }
    }
}