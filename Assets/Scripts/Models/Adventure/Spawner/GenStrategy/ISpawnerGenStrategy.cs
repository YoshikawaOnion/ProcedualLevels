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
        /// <param name="map"></param>
        /// <param name="spawnerParameter"></param>
        IEnumerable<Spawner> Generate(MapData map, SpawnerParameter spawnerParameter);
    }
}