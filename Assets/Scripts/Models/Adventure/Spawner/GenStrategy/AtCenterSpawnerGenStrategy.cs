using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// スポナーを部屋の中央に配置するアルゴリズムを提供するクラス。
    /// </summary>
    public class AtCenterSpawnerGenStrategy : ISpawnerGenStrategy
    {
        /// <summary>
        /// スポナーを生成します。
        /// </summary>
        /// <returns>生成したスポナーのコレクション。</returns>
        /// <param name="map"></param>
        /// <param name="spawnerParameter"></param>
        public IEnumerable<Spawner> Generate(MapData map, SpawnerParameter parameter)
        {
            var startX = map.StartLocation.x;
            var startY = map.StartLocation.y;
            var goalX = map.GoalLocation.x;
            var goalY = map.GoalLocation.y;
            foreach (var div in map.Divisions)
            {
                if (div.Room.IsInside(startX, startY)
                   || div.Room.IsInside(goalX, goalY))
                {
                    var leftBottom = new Vector2(div.Room.Left, div.Room.Bottom);
                    var rightTop = new Vector2(div.Room.Right, div.Room.Top);
                    var pos = (leftBottom + rightTop) / 2;
                    yield return new Spawner(parameter)
                    {
                        InitialPosition = pos
                    };
                }
            }
        }
    }
}