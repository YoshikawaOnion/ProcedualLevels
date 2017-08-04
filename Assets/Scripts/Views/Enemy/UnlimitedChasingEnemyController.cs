﻿using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class UnlimitedChasingEnemyController : EnemyController
    {
        private MoveController MoveController { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
			base.Initialize(enemy, context);
			MoveController = GetComponent<MoveController>();
        }

        public override void Control()
		{
			if (Context.Hero == null)
			{
                Rigidbody.velocity = Vector2.zero;
				return;
			}

			var direction = (Context.Hero.transform.position - transform.position).normalized;
			var walkDirection = Helper.Sign(direction.x, 0.2f);
			var x = MoveController.GetMoveSpeed(Rigidbody.velocity.x, walkDirection, 1);
			var y = MoveController.GetMoveSpeed(Rigidbody.velocity.y, walkDirection, 1);
			Rigidbody.velocity = new Vector2(x, y);
        }
    }
}