using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ゲームに登場するスポナーのデータを管理するクラス。
    /// </summary>
    public class SpawnerDatabase
    {
        /// <summary>
        /// ゲームに登場するスポナーの設定が入った配列を取得します。
        /// </summary>
        /// <value>The spawners.</value>
        public SpawnerParameter[] Spawners { get; private set; }

        public SpawnerDatabase()
        {
            var database = new EnemyDatabase();

            Spawners = new SpawnerParameter[]
            {
                new SpawnerParameter()
                {
                    Name = "GhostSpawner",
                    Generator = new AtCenterSpawnerGenStrategy(),
                    Behavior = new TimeLimitSpawnerBehavior(),
                    EnemiesAbility = database.Enemies[2],
                },
                new SpawnerParameter()
                {
                    Name = "SlimeSpawner",
                    Generator = new RandomSpawnerGenStrategy(),
                    Behavior = new TimeSpanSpawnerBehavior(TimeSpan.FromSeconds(20)),
                    EnemiesAbility = database.Enemies[0],
                },
            };
        }
    }
}