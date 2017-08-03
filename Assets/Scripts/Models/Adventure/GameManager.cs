using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using System;

namespace ProcedualLevels.Models
{
    public class GameManager
    {
        private CompositeDisposable Disposable { get; set; }

        public void Initialize(DungeonGenAsset dungeonAsset, GameParameterAsset gameAsset, IAdventureView view)
        {
            Disposable = new CompositeDisposable();

            var map = GenerateMap(dungeonAsset, gameAsset, view);
            var context = new AdventureContext()
            {
                Hero = new Hero(0, view)
                {
                    MaxHp = { Value = gameAsset.PlayerHp },
                    Hp = { Value = gameAsset.PlayerHp },
                    Attack = { Value = gameAsset.PlayerAttack }
                },
                Enemies = map.Enemies.ToArray(),
                Map = map,
                TimeLimit = new ReactiveProperty<int>(gameAsset.TimeLimitSeconds),
                NextBattlerIndex = map.Enemies.Max(x => x.Index) + 1,
                View = view
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
                nextViewStream.Subscribe(nextView => next.Initialize(dungeonAsset, gameAsset, nextView));
            })
                .AddTo(Disposable);
            
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Where(x => context.TimeLimit.Value > 0)
                      .Subscribe(x => context.TimeLimit.Value -= 1)
                      .AddTo(Disposable);
        }

        public MapData GenerateMap(DungeonGenAsset asset, GameParameterAsset battlerAsset, IAdventureView view)
        {
            var generator = new MapGenerator(battlerAsset, asset);
            var leftBottom = new Vector2(-asset.WorldWidth / 2, -asset.WorldHeight / 2);
            var rightTop = new Vector2(asset.WorldWidth / 2, asset.WorldHeight / 2);
            return generator.GenerateMap(leftBottom, rightTop, view);
        }
    }
}