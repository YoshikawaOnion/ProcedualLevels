using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// プレイヤーを見つけると追跡する敵キャラクターの振る舞いを提供するコンポーネント。
    /// </summary>
    public class ChasingEnemyController : FindingEnemyController
	{
        private MoveController MoveController { get; set; }
        private new Rigidbody2D Rigidbody { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            MoveController = GetComponent<MoveController>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        public override void ControlAtIdle()
        {
        }

        public override void ControlAtPlayerFound()
		{
			if (Context.Hero == null)
			{
				return;
			}

			var direction = (Context.Hero.transform.position - transform.position).normalized;
            var walkDirection = Helper.Sign(direction.x, 0.2f);
            var vx = MoveController.GetMoveSpeed(Rigidbody.velocity.x, walkDirection, 1);
            Rigidbody.velocity = Rigidbody.velocity.MergeX(vx);
        }
    }
}