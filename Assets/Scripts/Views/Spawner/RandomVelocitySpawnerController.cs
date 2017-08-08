using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class RandomVelocitySpawnerController : SpawnerController
    {
        [SerializeField]
        private float MaxSpeed = 2;

        public override void Initialize(Models.Spawner spawner, AdventureViewContext context)
        {
            spawner.SpawnObservable.Subscribe(x =>
            {
                var obj = context.Manager.SpawnEnemy(x);
                var vx = Helper.RandomInRange(-MaxSpeed, MaxSpeed);
                obj.Rigidbody.velocity = obj.Rigidbody.velocity.MergeX(vx);
            });
        }
    }
}