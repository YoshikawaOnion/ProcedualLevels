using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace ProcedualLevels.Models
{
    public class EnemyMasterAsset : ScriptableObject
    {
        [Serializable]
        public class EnemiesProperty
        {
            public string Name;
            public int Id;
            public int Hp;
            public int Attack;
            public int Score;
            public string PrefabName;
            public string GenStrategyName;
            public float FloatParameter1;
        }

        public List<EnemiesProperty> Enemies = new List<EnemiesProperty>();

        public IEnumerable<EnemiesAbility> GetEnemiesAbilities()
        {
            return Enemies.Select(GetEnemy);
        }

        private EnemiesAbility GetEnemy(EnemiesProperty property)
        {
            return new EnemiesAbility()
            {
                Id = property.Id,
                Hp = property.Hp,
                Attack = property.Attack,
                Score = property.Score,
                PrefabName = property.PrefabName,
                GenerationStrategy = GetGenStrategy(property),
            };
        }

        private EnemyGenStrategy GetGenStrategy(EnemiesProperty property)
        {
            switch (property.GenStrategyName)
            {
            case "Random":
                return new RandomEnemyGenStrategy(property.FloatParameter1);
            case "OnGoal":
                return new OnGoalEnemyGenStrategy();
            case "Null":
                return new NullEnemyGenStrategy();
            default:
                throw new ArgumentException("不明な EnemyGenStrategy");
            }
        }
    }
}
