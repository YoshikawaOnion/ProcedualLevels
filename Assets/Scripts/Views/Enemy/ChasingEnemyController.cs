using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// プレイヤーを見つけると追跡する敵キャラクターの振る舞いを提供するコンポーネント。
    /// </summary>
    public class ChasingEnemyController : FindingEnemyController
	{
        [SerializeField]
        private Collider2D FloorFinderLeft;
        [SerializeField]
        private Collider2D FloorFinderRight;
        [SerializeField]
        private int LeftFloorCountDebug;
        [SerializeField]
        private int RightFloorCountDebug;

        private MoveController MoveController { get; set; }
        private int LeftFloorCount { get; set; }
        private int RightFloorCount { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            MoveController = GetComponent<MoveController>();

            SubscribeFloorState(FloorFinderLeft, x => LeftFloorCount += x);
            SubscribeFloorState(FloorFinderRight, x => RightFloorCount += x);
        }

        private void SubscribeFloorState(Collider2D collider, Action<int> adder)
        {
            collider.OnTriggerEnter2DAsObservable()
                    .Where(x => x.tag == Def.TerrainTag || x.tag == Def.PlatformTag)
                    .Subscribe(x => adder(1));
            collider.OnTriggerExit2DAsObservable()
                    .Where(x => x.tag == Def.TerrainTag || x.tag == Def.PlatformTag)
                    .Subscribe(x => adder(-1));
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

            if (walkDirection > 0 && RightFloorCount == 0)
            {
                walkDirection = 0;
            }
            else if (walkDirection < 0 && LeftFloorCount == 0)
            {
                walkDirection = 0;
            }

            LeftFloorCountDebug = LeftFloorCount;
            RightFloorCountDebug = RightFloorCount;

            var vx = MoveController.GetMoveSpeed(Rigidbody.velocity.x, walkDirection, 1);
            Rigidbody.velocity = Rigidbody.velocity.MergeX(vx);
        }
    }
}