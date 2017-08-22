using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    public class AdventureFlow : IFlow
    {
        private CompositeDisposable Disposable { get; set; }
        private IAdventureView View { get; set; }
        private AdventureContext Context { get; set; }

        public void Initialize(DungeonGenAsset dungeonAsset, GameParameterAsset gameAsset, IAdventureView view)
        {
            Disposable = new CompositeDisposable();
            View = view;

            var map = GenerateMap(dungeonAsset, gameAsset, view);
            Context = new AdventureContext()
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
            map.Spawners.ForEach(x => x.Initialize(Context));

            foreach (var enemy in map.Enemies)
            {
                enemy.IsAlive.Where(x => !x)
                     .Subscribe(x => Context.Score.Value += 1);
            }

            view.Initialize(Context);
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

        private IEnumerator Run(IObserver<IFlow> result)
        {
            // 残り時間を減らしていく
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Where(x => Context.TimeLimit.Value > 0)
                      .Subscribe(x => Context.TimeLimit.Value -= 1)
                      .AddTo(Disposable);
            
            yield return View.OnGoal.Merge(View.OnPlayerDie)
                             .Take(1)
                             .ToYieldInstruction();

            yield return Observable.Timer(TimeSpan.FromSeconds(2))
                                   .ToYieldInstruction();

            Context.Dispose();
            Disposable.Dispose();

            IAdventureView nextView = null;
            yield return View.ResetAsync()
                             .Do(x => nextView = x)
                             .ToYieldInstruction();

            var next = new AdventureFlow();
            next.Initialize(AssetRepository.I.DungeonGenAsset,
                            AssetRepository.I.GameParameterAsset,
                            nextView);
            result.OnNext(next);
            result.OnCompleted();
        }

        public IObservable<IFlow> Start()
        {            
            return Observable.FromCoroutine<IFlow>(observer => Run(observer));
        }
    }
}