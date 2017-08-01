using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class NullEnemyGenStrategy : EnemyGenStrategy
    {
        public override void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index)
        {
        }
    }
}