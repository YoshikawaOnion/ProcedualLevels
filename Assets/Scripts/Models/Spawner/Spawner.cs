using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class Spawner : IDisposable
    {
        public IObservable<Enemy> SpawnObservable { get; private set; }
        public Vector2 InitialPosition { get; set; }

        private ISpawnerBehavior Behavior { get; set; }
        private EnemiesAbility Ability { get; set; }
        private IDisposable Disposable { get; set; }

        public Spawner(ISpawnerBehavior behavior, EnemiesAbility ability)
        {
            Behavior = behavior;
            Ability = ability;
        }

        public void Initialize(AdventureContext context)
        {
            SpawnObservable = Behavior.GetSpawnStream(context)
                                      .Select(x => new Enemy(context.NextBattlerIndex,
                                                             InitialPosition,
                                                             Ability,
                                                             context.View));
            Disposable = SpawnObservable.Subscribe(x => ++context.NextBattlerIndex);
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}