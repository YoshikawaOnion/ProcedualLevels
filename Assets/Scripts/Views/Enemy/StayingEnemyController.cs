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
        private Rigidbody2D Rigidbody { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            InitialPosition = transform.position;
            MoveController = GetComponent<MoveController>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        public override void ControlAtIdle()
        {
            Move();
        }

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