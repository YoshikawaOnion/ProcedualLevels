using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 敵を一体も生成しないアルゴリズムを提供します。
    /// </summary>
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