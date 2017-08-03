using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    public class SpawnerController : MonoBehaviour
    {
        private IDisposable Disposable { get; set; }

        public void Initialize(Models.Spawner spawner, AdventureViewContext context)
		{
			Disposable = spawner.SpawnObservable.Subscribe(x =>
            {
                context.SpawnEnemy(x);
            });            
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
        }
    }
}