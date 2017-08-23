using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    public class SpawnerMasterAsset : ScriptableObject
    {
        [System.Serializable]
        public class SpawnerProperty
        {
            public string Name;
            public string PrefabName;
            public string GeneratorName;
            public string BehaviorName;
            public int EnemyId;
            public float FloatParameter1;
        }

        public List<SpawnerProperty> Properties = new List<SpawnerProperty>();

        public IEnumerable<SpawnerParameter> GetParameters()
        {
            var enemies = AssetRepository.I.EnemyMaster.GetEnemiesAbilities();
            return Properties.Select(x => new SpawnerParameter
            {
                PrefabName = x.PrefabName,
                Generator = GetGenStrategy(x),
                Behavior = GetBehavior(x),
                EnemiesAbility = enemies.First(y => x.EnemyId == y.Id),
            });
        }

        private ISpawnerGenStrategy GetGenStrategy(SpawnerProperty property)
        {
            switch (property.GeneratorName)
            {
            case "AtCenter":
                return new AtCenterSpawnerGenStrategy();
            case "Random":
                return new RandomSpawnerGenStrategy();
            default:
                throw new System.ArgumentException("不明な SpawnerGenStrategy");
            }
        }

        private ISpawnerBehavior GetBehavior(SpawnerProperty property)
        {
            switch (property.BehaviorName)
            {
            case "TimeLimit":
                return new TimeLimitSpawnerBehavior();
            case "TimeSpan":
                return new TimeSpanSpawnerBehavior(TimeSpan.FromSeconds(property.FloatParameter1));
            default:
                throw new ArgumentException("不明な SpawnerBehavior");
            }
        }
    }
}
