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
                Enemies = map.Enemies,
                Map = map,
                TimeLimit = new ReactiveProperty<int>(gameAsset.TimeLimitSeconds),
                NextBattlerIndex = map.Enemies.Max(x => x.Index) + 1,
                View = view,
                Spawners = map.Spawners.ToArray()
            };
            map.Spawners.ForEach(x => x.Initialize(context));
            view.Initialize(context);

            // プレイヤーがゴールするか死んだらリセット
            view.OnGoal.Merge(view.OnPlayerDie)
                .SelectMany(x => Observable.Timer(TimeSpan.FromSeconds(2)))
                .First()
                .Subscribe(x =>
            {
                context.Dispose();
                Disposable.Dispose();
                var next = new GameManager();
                var nextViewStream = view.ResetAsync();
                nextViewStream.Subscribe(nextView => next.Initialize(dungeonAsset, gameAsset, nextView));
            })
                .AddTo(Disposable);

            // 残り時間を減らしていく
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Where(x => context.TimeLimit.Value > 0)
                      .Subscribe(x => context.TimeLimit.Value -= 1)
                      .AddTo(Disposable);
        }

        /// <summary>
        /// マップデータを生成します。
        /// </summary>
        /// <returns>生成したマップデータ。</returns>
        /// <param name="asset">GameParameterAsset。</param>
        /// <param name="battlerAsset">Battler asset.</param>
        /// <param name="view">View.</param>
        public MapData GenerateMap(DungeonGenAsset asset, GameParameterAsset battlerAsset, IAdventureView view)
        {
            var generator = new MapGenerator(battlerAsset, asset);
            var leftBottom = new Vector2(-asset.WorldWidth / 2, -asset.WorldHeight / 2);
            var rightTop = new Vector2(asset.WorldWidth / 2, asset.WorldHeight / 2);
            return generator.GenerateMap(leftBottom, rightTop, view);
        }
    }
}