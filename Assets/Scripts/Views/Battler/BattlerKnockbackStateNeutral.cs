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

        protected override void Control()
        {
            Context.Control();
        }

        public override void Knockback(KnockbackInfo info, BattlerController against)
        {
            var direction = -(against.transform.position - Context.transform.position)
                .MergeY(0)
                .normalized;
            var force = direction * info.KnockbackPower;
            var jump = new Vector3(0, info.KnockbackJumpPower, 0);

            Context.Rigidbody.velocity = Vector3.zero;
            Context.Rigidbody.AddForce(force + jump);

            ChangeState(new BattlerKnockbackStateKnockback(Context, info));

            var jumpState = Context.GetComponent<HeroMoveController>();
            if (jumpState != null)
            {
                jumpState.SetJumpState();
            }
        }
    }
}