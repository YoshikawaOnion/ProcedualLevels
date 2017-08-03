using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class GameManager
    {
        public void Initialize(DungeonGenAsset dungeonGen, BattlerGenAsset battlerGen, IAdventureView view)
        {
            var map = GenerateMap(dungeonGen, battlerGen, view);
            var context = new AdventureContext()
            {
                Hero = new Hero(0, view)
                {
                    MaxHp = { Value = battlerGen.PlayerHp },
                    Hp = { Value = battlerGen.PlayerHp },
                    Attack = { Value = battlerGen.PlayerAttack }
                },
                Enemies = map.Enemies.ToArray(),
                Map = map,
            };
            view.Initialize(context);

            view.OnGoal.Merge(view.OnPlayerDie)
                .SelectMany(x => Observable.Timer(TimeSpan.FromSeconds(2)))
                .First()
                .Subscribe(x =>
            {
                context.Dispose();
                var next = new GameManager();
                var nextViewStream = view.ResetAsync();
                nextViewStream.Subscribe(nextView => next.Initialize(dungeonGen, battlerGen, nextView));
            });
        }

        public MapData GenerateMap(DungeonGenAsset asset, BattlerGenAsset battlerAsset, IAdventureView view)
        {
            var generator = new MapGenerator(battlerAsset, asset);
            var leftBottom = new Vector2(-asset.WorldWidth / 2, -asset.WorldHeight / 2);
            var rightTop = new Vector2(asset.WorldWidth / 2, asset.WorldHeight / 2);
            return generator.GenerateMap(leftBottom, rightTop, view);
        }
    }
}