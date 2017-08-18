using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System.Linq;
using ProcedualLevels.Common;
using System;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// プレイヤーの戦闘を管理するクラス。
    /// </summary>
    public class HeroBattleController : MonoBehaviour
    {
        private List<EnemyController> BattleTargets { get; set; }
        private IPlayerEventAccepter EventAccepter { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private HeroAnimationController Animation { get; set; }
        private HeroMoveController Move { get; set; }
        private Subject<Unit> OnBattle { get; set; }
        private GameObject DamageEffectPrefab { get; set; }

        public void Initialize(IPlayerEventAccepter eventAccepter, IGameEventReceiver eventReceiver)
        {
            BattleTargets = new List<EnemyController>();
            EventAccepter = eventAccepter;
            Disposable = new CompositeDisposable();
            Animation = GetComponent<HeroAnimationController>();
            Move = GetComponent<HeroMoveController>();
            OnBattle = new Subject<Unit>();
            DamageEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/HitEffect01");
            var collider = GetComponent<BoxCollider2D>();

            var onTouchingEnemy = collider.OnCollisionStay2DAsObservable()
                                          .Select(x => x.gameObject.GetComponent<EnemyController>())
                                          .Where(x => x != null);

            // 次フレームの戦闘処理の対象として登録。戦闘をした後しばらくは登録しない
            onTouchingEnemy.PauseBy(OnBattle, TimeSpan.FromMilliseconds(750))
                           .Subscribe(x => BattleTargets.Add(x))
                           .AddTo(Disposable);

            // 戦闘をしたら戦闘アニメーションを再生
            onTouchingEnemy.Where(IsAttackingFor)
                           .DistinctUntilChanged(x => Move.WalkDirection)
                           .Subscribe(x => Animation.AnimateAttack(x.gameObject));
            
            // 少しの間戦闘をしないでいると通常のアニメーションへ遷移
            onTouchingEnemy.Where(IsAttackingFor)
                           .Throttle(TimeSpan.FromMilliseconds(200))
                           .Subscribe(x => Animation.AnimateNeutral())
                           .AddTo(Disposable);
            
            var hero = GetComponent<HeroController>();

            // トゲに当たったらダメージ。一度当たったら少しの間トゲのダメージを受けない
            eventReceiver.OnBattlerTouchedSpikeReceiver
                         .Where(x => x.Item2.Battler.Index == hero.Battler.Index)
                         .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                         .Subscribe(x => 
            {
                Animation.AnimateDamage(x.Item1.gameObject);
                PlayHitEffect(x.Item1.transform.position);
            });
        }

        private bool IsAttackingFor(EnemyController x)
        {
            var distance = x.transform.position - transform.position;
            return distance.x * Move.WalkDirection > 0;
        }

        void Update()
        {
            if (BattleTargets.Any())
            {
                var distincted = BattleTargets.GroupBy(x => x.Battler.Index)
                                              .Select(x => x.First());
                foreach (var target in distincted)
                {
                    if (IsAttackingFor(target))
                    {
                        EventAccepter.OnPlayerBattleWithEnemySender
                                     .OnNext(target.Enemy);
                    }
                    else
                    {
                        EventAccepter.OnPlayerAttackedByEnemySender
                                     .OnNext(target.Enemy);
                        Animation.AnimateDamage(target.gameObject);
                    }
                    PlayHitEffect(target.transform.position);
                }

                BattleTargets.Clear();
                OnBattle.OnNext(Unit.Default);
            }
        }

        private void PlayHitEffect(Vector3 oppositeLocation)
        {
            var obj = Instantiate(DamageEffectPrefab);
            obj.transform.position = ((transform.position + oppositeLocation) / 2)
                .MergeZ(obj.transform.position.z);
            obj.UpdateAsObservable()
               .Skip(60)
               .Subscribe(x => Destroy(obj))
               .AddTo(Disposable);
        }
    }
}