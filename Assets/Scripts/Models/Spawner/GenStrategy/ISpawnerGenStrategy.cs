using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface ISpawnerGenStrategy
    {
        IEnumerable<Spawner> Generate(MapData map, ISpawnerBehavior behavior, EnemiesAbility ability);
    }
}