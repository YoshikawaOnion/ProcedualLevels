using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 敵キャラクターを生成するアルゴリズムを提供するクラスの基底クラス。
    /// </summary>
    public abstract class EnemyGenStrategy
    {
        public abstract void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index);
    }
}