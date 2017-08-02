using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class EnemyDatabase
    {
        public EnemiesAbility[] Enemies;

        public EnemyDatabase()
        {
            var battlerGen = Resources.Load<BattlerGenAsset>(Def.BattlerGenAssetPath);
            var dungeonGen = Resources.Load<DungeonGenAsset>(Def.DungeonGenAssetPath);
            Enemies = new EnemiesAbility[]
            {
                new EnemiesAbility()
                {
                    Id = 0,
                    Hp = 5,
                    Attack = 1,
                    PrefabName = "Enemy_Control",
                    GenerationStrategy = new RandomEnemyGenStrategy(0.05f, battlerGen, dungeonGen),
                },
                new EnemiesAbility()
                {
                    Id = 1,
                    Hp = 40,
                    Attack = 3,
                    PrefabName = "Boss_Control",
                    GenerationStrategy = new OnGoalEnemyGenStrategy(),
                },
            };
        }
    }
}