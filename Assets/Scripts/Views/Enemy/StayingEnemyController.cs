using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class StayingEnemyController : FindingEnemyController
    {
        private Vector2 InitialPosition { get; set; }
        private WalkController WalkController { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            InitialPosition = transform.position;
            WalkController = GetComponent<WalkController>();
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
            var walkDirection = Helper.Sign(direction.x);
            WalkController.Walk(walkDirection, 1);
        }
    }
}