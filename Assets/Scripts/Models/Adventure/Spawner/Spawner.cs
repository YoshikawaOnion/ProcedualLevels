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
        /// <summary>
        /// 敵をスポーンさせるタイミングを通知するストリームを取得します。
        /// </summary>
        public IObservable<Enemy> SpawnObservable { get; private set; }
        /// <summary>
        /// このスポナーの初期位置を取得または設定します。
        /// </summary>
        public Vector2 InitialPosition { get; set; }
        /// <summary>
        /// このスポナーに対するプレハブ名を取得または設定します。
        /// </summary>
        /// <value>The name of the prefab.</value>
        public string PrefabName { get; set; }

        private ISpawnerBehavior Behavior { get; set; }
        private EnemiesAbility Ability { get; set; }
        private IDisposable Disposable { get; set; }

        /// <summary>
        /// パラメーターを指定して、<see cref="T:ProcedualLevels.Models.Spawner"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="spawnerParameter">スポナーのパラメーター。</param>
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