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
            var battlerGen = Resources.Load<GameParameterAsset>(Def.GameParameterAssetPath);
            var dungeonGen = Resources.Load<DungeonGenAsset>(Def.DungeonGenAssetPath);
            Enemies = new EnemiesAbility[]
            {
                new EnemiesAbility()
                {
                    Id = 0,
                    Hp = 5,
                    Attack = 3,
                    PrefabName = "Enemy_Control",
                    GenerationStrategy = new RandomEnemyGenStrategy(0.05f, battlerGen, dungeonGen),
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