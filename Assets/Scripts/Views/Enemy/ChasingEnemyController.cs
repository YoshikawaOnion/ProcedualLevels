using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class ChasingEnemyController : FindingEnemyController
	{
        private WalkController WalkController { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);
            WalkController = GetComponent<WalkController>();
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
            WalkController.Walk(walkDirection, 1);
        }
    }
}