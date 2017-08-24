using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class TreasureGenerator
    {
        public void PlaceTreasures(MapData map, AdventureContext context)
        {
            var colliderMargin = AssetRepository.I.DungeonGenAsset.ColliderMargin;
            foreach (var item in map.Divisions)
            {
                var random = UnityEngine.Random.value;
                if (random <= 0.29f)
                {
                    var location = Helper.GetRandomLocation(item.Room, colliderMargin);
                    var treasure = new Treasure(location, context);
                    map.Treasures.Add(treasure);
                }
            }
        }
    }
}