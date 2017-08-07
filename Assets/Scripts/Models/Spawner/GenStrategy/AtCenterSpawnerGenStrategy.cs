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
        public IEnumerable<Spawner> Generate(MapData map, ISpawnerBehavior behavior, EnemiesAbility ability)
		{
			foreach (var div in map.Divisions)
			{
				var leftBottom = new Vector2(div.Room.Left, div.Room.Bottom);
				var rightTop = new Vector2(div.Room.Right, div.Room.Top);
				var pos = (leftBottom + rightTop) / 2;
                yield return new Spawner(behavior, ability)
                {
                    InitialPosition = pos
                };
			}
        }
    }
}