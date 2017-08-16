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
            Disposable = spawner.SpawnObservable.Subscribe(x =>
            {
                var obj = context.ObjectManager.SpawnEnemy(x);
                var vx = Helper.GetRandomInRange(-MaxSpeed, MaxSpeed);
                obj.Rigidbody.velocity = obj.Rigidbody.velocity.MergeX(vx);
            });
        }
    }
}