using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// スポナーを表すクラス。
    /// </summary>
    public class Spawner : IDisposable
    {
        public IObservable<Enemy> SpawnObservable { get; private set; }
        public Vector2 InitialPosition { get; set; }
        public string PrefabName { get; set; }

        private ISpawnerBehavior Behavior { get; set; }
        private EnemiesAbility Ability { get; set; }
        private IDisposable Disposable { get; set; }

        public Spawner(SpawnerParameter spawnerParameter)
        {
            PrefabName = spawnerParameter.PrefabName;
            Behavior = spawnerParameter.Behavior;
            Ability = spawnerParameter.EnemiesAbility;
        }

        public void Initialize(AdventureContext context)
        {
            SpawnObservable = Behavior.GetSpawnStream(context)
                                      .Select(x => new Enemy(context.NextBattlerIndex,
                                                             InitialPosition,
                                                             Ability,
                                                             context.View))
                                      .Do(x => context.NextBattlerIndex++);
            Disposable = SpawnObservable.Subscribe(x =>
            {
                context.Enemies.Add(x);
            });
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}