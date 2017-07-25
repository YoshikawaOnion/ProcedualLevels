using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class EnemyController : BattlerController
    {
        [SerializeField]
        public float WalkSpeed;

        public Models.Enemy Enemy { get; private set; }
        public EnemyFindState FindState { get; set; }

        public void Initialize(Models.Enemy enemy, IGameEventReceiver eventReceiver)
        {
            base.Initialize(enemy);
            Enemy = enemy;
            FindState = new EnemyFindStateLookingFor(this);
            FindState.Subscribe();

            var animation = GetComponent<EnemyAnimationController>();
            animation.Initialize(eventReceiver);
        }
		
		public override void Control()
		{
			FindState.Control();
		}
    }   
}
