using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    public enum AdventureResult
    {
        Success, Miss,
    }

    public class AdventureFlow : IFlow
    {
        private CompositeDisposable Disposable { get; set; }
        private IAdventureView View { get; set; }
        private AdventureContext Context { get; set; }

        public void Initialize(IAdventureView view)
        {
            var dungeonAsset = AssetRepository.I.DungeonGenAsset;
            var gameAsset = AssetRepository.I.GameParameterAsset;

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
                Map = map,
                TimeLimit = new ReactiveProperty<int>(gameAsset.TimeLimitSeconds),
                NextBattlerIndex = map.Enemies.Max(x => x.Index) + 1,
                View = view,
                Spawners = map.Spawners.ToArray()
            };
            map.Spawners.ForEach(x => x.Initialize(Context));

            var treasureGen = new TreasureGenerator();
            treasureGen.PlaceTreasures(map, Context);

            foreach (var enemy in map.Enemies)
            {
                Context.AddEnemy(enemy);
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

            AdventureResult r = AdventureResult.Success;
            var goal = View.OnGoal.Select(x => AdventureResult.Success);
            var die = View.OnPlayerDie.Select(x => AdventureResult.Miss);

            yield return goal.Merge(die)
                             .Take(1)
                             .Do(x => r = x)
                             .ToYieldInstruction();

            if (r == AdventureResult.Miss)
            {
                yield return Observable.TimerFrame(30)
                                       .ToYieldInstruction();
                
                View.StopGame();

                yield return Observable.TimerFrame(60)
                                       .ToYieldInstruction();

                Context.Dispose();
                Disposable.Dispose();

                IAdventureView nextView = null;
                yield return View.ResetAsync()
                                 .Do(x => nextView = x)
                                 .ToYieldInstruction();

                var next = new AdventureFlow();
                next.Initialize(nextView);
                result.OnNext(next);
                result.OnCompleted();
            }
            else if(r == AdventureResult.Success)
            {
                View.StopGame();

                yield return Observable.TimerFrame(60)
                                       .ToYieldInstruction();

                Context.Dispose();
                Disposable.Dispose();

                IResultView nextView = null;
                yield return View.GotoResult(Context.TimeLimit.Value, Context.Score.Value)
                                 .Do(x => nextView = x)
                                 .ToYieldInstruction();

                var next = new ResultFlow(Context.TimeLimit.Value, Context.Score.Value, nextView);
                result.OnNext(next);
                result.OnCompleted();
            }
        }

        public IObservable<IFlow> Start()
        {            
            return Observable.FromCoroutine<IFlow>(observer => Run(observer));
        }
    }
}