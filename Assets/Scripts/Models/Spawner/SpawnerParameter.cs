using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class SpawnerParameter
    {
        public string PrefabName { get; set; }
        public ISpawnerGenStrategy Generator { get; set; }
        public ISpawnerBehavior Behavior { get; set; }
        public EnemiesAbility EnemiesAbility { get; set; }

        public IEnumerable<Spawner> Generate(MapData map)
        {
            return Generator.Generate(map, this);
        }
    }
}