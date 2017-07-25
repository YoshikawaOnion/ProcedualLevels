using UnityEngine;
using System.Collections;
using System.Linq;

namespace ProcedualLevels.Models
{
    public class GameManager
    {
        public void Initialize(DungeonGenAsset asset, IAdventureView view)
        {
            var context = new AdventureContext()
            {
                Hero = new Hero(0, view),
                Map = GenerateMap(asset)
            };
            context.Enemeis = context.Map.EnemyLocations
                .Select((x, i) => new Enemy(i + 1, x, view))
                .ToArray();
            view.Initialize(context);
        }

        public MapData GenerateMap(DungeonGenAsset asset)
        {
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