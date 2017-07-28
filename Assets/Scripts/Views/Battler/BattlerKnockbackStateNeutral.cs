using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
	/// <summary>
	/// ノックバックをしていない状態の機能を提供します。
	/// </summary>
	public class BattlerKnockbackStateNeutral : BattlerKnockbackState
	{
		private static readonly float KnockbackFactor = 150;
		private static readonly float KnockbackJumpPower = 100;

        public BattlerKnockbackStateNeutral(BattlerController context) : base(context)
        {
        }

        protected override void Control()
        {
            Context.Control();
        }

        public override void Knockback(BattlerController against, int power)
        {
            var direction = -(against.transform.position - Context.transform.position)
                .MergeY(0)
                .normalized;
            var force = direction * power * KnockbackFactor;
            var jump = new Vector3(0, KnockbackJumpPower, 0);

            Context.Rigidbody.velocity = Vector3.zero;
            Context.Rigidbody.AddForce(force + jump);

            ChangeState(new BattlerKnockbackStateKnockback(Context, power));

            var jumpState = Context.GetComponent<HeroMoveController>();
            if (jumpState != null)
            {
                jumpState.SetJumpState();
            }
        }
    }
}