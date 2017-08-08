using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ランダムな位置にスポナーを生成するアルゴリズムを提供します。
    /// </summary>
    public class RandomSpawnerGenStrategy : ISpawnerGenStrategy
    {
        public IEnumerable<Spawner> Generate(MapData map, SpawnerParameter spawnerParameter)
        {
            foreach (var div in map.Divisions)
            {
                var location = Helper.GetRandomLocation(div.Room, 1);
                yield return new Spawner(spawnerParameter)
                {
                    InitialPosition = location
                };
            }
        }
    }
}