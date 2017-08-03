using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ProcedualLevels.Views
{
    public class SpawnerEnemyController : EnemyController
    {
        private bool SpawningActivated { get; set; }

        public override void Initialize(Models.Enemy enemy, AdventureViewContext context)
        {
            base.Initialize(enemy, context);

            SpawningActivated = false;

            context.Model.TimeLimit
                   .FirstOrDefault(x => x <= 0)
                   .SelectMany(Observable.Interval(TimeSpan.FromSeconds(60)))
                   .Subscribe();
        }

        public override void Control()
        {
        }
    }
}