﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// スポナーからの敵のスポーン方法を提供するクラス。
    /// </summary>
    public class SpawnerController : MonoBehaviour
    {
        protected IDisposable Disposable { get; set; }

        public virtual void Initialize(Models.Spawner spawner, AdventureViewContext context)
		{
            Disposable = spawner.SpawnObservable
                                .Subscribe(x => context.ObjectManager.SpawnEnemy(x));
        }

        private void OnDestroy()
        {
            Disposable.Dispose();
        }
    }
}