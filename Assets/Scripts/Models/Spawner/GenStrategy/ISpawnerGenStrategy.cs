using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// スポナーを生成するアルゴリズムを提供するインターフェース。
    /// </summary>
    public interface ISpawnerGenStrategy
    {
        /// <summary>
        /// スポナーを生成します。
        /// </summary>
        /// <returns>生成したスポナーのコレクション。</returns>
        /// <param name="map">スポナーが所属するマップ。</param>
        /// <param name="behavior">スポナーの振る舞いを提供するクラスのインスタンス。</param>
        /// <param name="ability">スポナーから現れる敵キャラクターのパラメータ。</param>
        IEnumerable<Spawner> Generate(MapData map, ISpawnerBehavior behavior, EnemiesAbility ability);
    }
}