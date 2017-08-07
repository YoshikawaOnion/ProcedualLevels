using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 登場する敵キャラクターのデータを管理するクラス。
    /// </summary>
    public class EnemyDatabase
    {
        /// <summary>
        /// 登場する敵キャラクターのデータを並べた配列。
        /// </summary>
        public EnemiesAbility[] Enemies;

        public EnemyDatabase()
        {
            Enemies = new EnemiesAbility[]
            {
                new EnemiesAbility()
                {
                    Id = 0,
                    Hp = 5,
                    Attack = 3,
                    PrefabName = "Enemy_Control",
                    GenerationStrategy = new RandomEnemyGenStrategy(0.05f),
                },
                new EnemiesAbility()
                {
                    Id = 1,
                    Hp = 40,
                    Attack = 5,
                    PrefabName = "Boss_Control",
                    GenerationStrategy = new OnGoalEnemyGenStrategy(),
                },
                new EnemiesAbility()
                {
                    Id = 2,
                    Hp = 100,
                    Attack = 20,
                    PrefabName = "Ghost_Control",
                    GenerationStrategy = new NullEnemyGenStrategy(),
                },
            };
        }
    }
}