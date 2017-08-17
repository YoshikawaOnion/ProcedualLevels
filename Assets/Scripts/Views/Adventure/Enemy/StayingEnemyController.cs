using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// その場に留まろうとする敵キャラクターの振る舞いを提供します。
    /// </summary>
    public class StayingEnemyController : FindingEnemyController
    {
        private Vector2 InitialPosition { get; set; }
        private MoveController MoveController { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            InitialPosition = transform.position;
            MoveController = GetComponent<MoveController>();
        }

        /// <summary>
        /// オーバーライドされることで、プレイヤーを発見する前に毎フレーム実行される処理を実行します。
        /// </summary>
        public override void ControlAtIdle()
        {
            Move();
        }

        /// <summary>
        /// オーバーライドされることで、プレイヤーを発見した後に毎フレーム実行される処理を実行します。
        /// </summary>
        public override void ControlAtPlayerFound()
        {
            Move();
        }

        private void Move()
        {
            var direction = InitialPosition.ToVector3() - transform.position;
            var walkDirection = Helper.Sign(direction.x, 0.2f);
            var vx = MoveController.GetMoveSpeed(Rigidbody.velocity.x, walkDirection, 1);
            Rigidbody.velocity = Rigidbody.velocity.MergeX(vx);
        }
    }
}