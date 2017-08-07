using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class RandomSpawnerGenStrategy : ISpawnerGenStrategy
    {
        public IEnumerable<Spawner> Generate(MapData map,
                                             ISpawnerBehavior behavior,
                                             EnemiesAbility ability)
        {
            foreach (var div in map.Divisions)
            {
                var location = Helper.GetRandomLocation(div.Room, 1);
                yield return new Spawner(behavior, ability)
                {
                    InitialPosition = location
                };
            }
        }
    }
}