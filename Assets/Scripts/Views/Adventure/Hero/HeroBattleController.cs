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
        private HeroController Hero { get; set; }

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
            Hero = GetComponent<HeroController>();

            var onTouchingEnemy = collider.OnCollisionStay2DAsObservable()
                                          .Select(x => x.gameObject.GetComponent<EnemyController>())
                                          .Where(x => x != null);

            // 次フレームの戦闘処理の対象として登録。戦闘をした後しばらくは登録しない
            onTouchingEnemy.PauseBy(OnBattle, TimeSpan.FromMilliseconds(750))
                           .Subscribe(x => BattleTargets.Add(x))
                           .AddTo(Disposable);

            // 少しの間戦闘をしないでいると通常のアニメーションへ遷移するためのストリーム
            var idleStream = onTouchingEnemy.Where(x => IsAttackingFor(x, Hero))
                                            .Throttle(TimeSpan.FromMilliseconds(500));

            // 攻撃方向を変えたり戦闘をやめたら攻撃アニメーションをリセットするためのストリーム
            var attackAnimationUpdateStream = onTouchingEnemy.DistinctUntilChanged(x => Hero.WalkDirection.Value)
                                                             .Skip(1)
                                                             .Merge(idleStream);
            
            // 戦闘をしたら戦闘アニメーションを再生。
            onTouchingEnemy.Where(x => IsAttackingFor(x, Hero))
                           .FirstOrDefault()
                           .Where(x => x != null)
                           .RepeatAt(attackAnimationUpdateStream)
                           .Subscribe(x => Animation.AnimateAttack(x.gameObject,
                                                                   Def.AttackAnimationPriority,
                                                                   Def.AttackAnimationPriority))
                           .AddTo(Disposable);
            
            idleStream.Subscribe(x => Animation.AnimateNeutral(Def.AttackAnimationPriority,
                                                               Def.MoveAnimationPriority))
                      .AddTo(Disposable);

            // トゲに当たったらダメージ。一度当たったら少しの間トゲのダメージを受けない
            eventReceiver.OnBattlerTouchedSpikeReceiver
                         .Where(x => x.Item2.Battler.Index == Hero.Battler.Index)
                         .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                         .Subscribe(x => 
            {
                Animation.AnimateDamage(x.Item1.gameObject);
                PlayHitEffect(x.Item1.transform.position);
            });
        }

        private bool IsAttackingFor(EnemyController x, HeroController hero)
        {
            var distance = x.transform.position - transform.position;
            return distance.x * hero.WalkDirection.Value > 0;
        }

        void Update()
        {
            if (BattleTargets.Any())
            {
                var distincted = BattleTargets.GroupBy(x => x.Battler.Index)
                                              .Select(x => x.First());
                BattlerController attackingFor = null;
                foreach (var target in distincted)
                {
                    if (IsAttackingFor(target, Hero))
                    {
                        EventAccepter.OnPlayerBattleWithEnemySender
                                     .OnNext(target.Enemy);
                        attackingFor = target;
                    }
                    else
                    {
                        EventAccepter.OnPlayerAttackedByEnemySender
                                     .OnNext(target.Enemy);
                    }
                    PlayHitEffect(target.transform.position);
                }

                if (attackingFor == null)
                {
                    Animation.AnimateDamage(BattleTargets[0].gameObject);
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

        private void OnDestroy()
        {
            Disposable.Dispose();
        }
    }
}