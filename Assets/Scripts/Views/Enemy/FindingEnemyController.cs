using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public abstract class FindingEnemyController : EnemyController
	{
        public EnemyFindState FindState { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
			base.Initialize(enemy, context);
			FindState = new EnemyFindStateLookingFor(this);
			FindState.Subscribe();
        }

        public override void Control()
        {
            FindState.Control();
        }

        public abstract void ControlAtPlayerFound();
        public abstract void ControlAtIdle();
    }
}