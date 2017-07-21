using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class GameManager
    {
        public MapData GenerateMap()
        {
            var asset = Resources.Load<DungeonGenAsset>
                                 ("Assets/DungeonGenAsset");

            var generator = new MapGenerator()
            {
                ChildBoundMinSize = asset.ChildBoundMinSize,
                ParentBoundMinSize = asset.ParentBoundMinSize,
                MarginSize = asset.MarginSize,
                PathThickness = asset.PathThickness,
                RoomMinSize = asset.RoomMinSize,
                RoomMaxSize = asset.RoomMaxSize,
                PathReducingChance = asset.PathReducingChance,
                EnemyCountRatio = asset.EnemyCountRatio,
            };
            var leftBottom = new Vector2(-asset.WorldWidth / 2, -asset.WorldHeight / 2);
            var rightTop = new Vector2(asset.WorldWidth / 2, asset.WorldHeight / 2);
            return generator.GenerateMap(leftBottom, rightTop);
        }
    }
}