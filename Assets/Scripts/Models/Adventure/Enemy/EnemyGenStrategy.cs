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
        /// <summary>
        /// 敵キャラクターを指定したマップに配置します。
        /// </summary>
        /// <param name="map">敵を配置する先のマップデータ。</param>
        /// <param name="ability">配置する敵のパラメータ。</param>
        /// <param name="view">探検画面のビュー。</param>
        /// <param name="index">新しいキャラクターのインデックス。</param>
        public abstract void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index);
    }
}