using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public class SpawnerController : MonoBehaviour
    {
        public virtual void Initialize(Models.Spawner spawner, AdventureViewContext context)
		{
            spawner.SpawnObservable.Subscribe(x => context.Manager.SpawnEnemy(x));
        }
    }
}