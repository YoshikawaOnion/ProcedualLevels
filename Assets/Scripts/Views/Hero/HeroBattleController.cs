﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System.Linq;
using ProcedualLevels.Common;
using System;

namespace ProcedualLevels.Views
{
    public class HeroBattleController : MonoBehaviour
    {
        private List<EnemyController> BattleTargets { get; set; }
        private IPlayerEventAccepter EventAccepter { get; set; }
		private CompositeDisposable Disposable { get; set; }
		private HeroAnimationController Animation { get; set; }
		private HeroMoveController Move { get; set; }
        private Subject<Unit> OnBattle { get; set; }

        public void Initialize(IPlayerEventAccepter eventAccepter, IGameEventReceiver eventReceiver)
        {
            BattleTargets = new List<EnemyController>();
            EventAccepter = eventAccepter;
            Disposable = new CompositeDisposable();
            Animation = GetComponent<HeroAnimationController>();
            Move = GetComponent<HeroMoveController>();
            OnBattle = new Subject<Unit>();
            var collider = GetComponent<BoxCollider2D>();

            var onTouchingEnemy = collider.OnCollisionStay2DAsObservable()
                                          .Select(x => x.gameObject.GetComponent<EnemyController>())
                                          .Where(x => x != null);
            
            onTouchingEnemy.PauseBy(OnBattle, TimeSpan.FromMilliseconds(750))
                           .Subscribe(x => BattleTargets.Add(x))
                           .AddTo(Disposable);

            // 敵を押し込んでいる間は攻撃アニメを繰り返し再生する
            onTouchingEnemy.FirstOrDefault(x => IsAttackingFor(x))
                           .SelectMany(x => GetSwingingLoop(x))
                           .TakeWhile(x => x != null && IsAttackingFor(x))
                           .RepeatSafe()
                           .Subscribe(x => Animation.AnimateAttack(x.gameObject))
                           .AddTo(Disposable);

            var hero = GetComponent<HeroController>();
            eventReceiver.OnBattlerTouchedSpikeReceiver
                         .Where(x => x.Item2.Battler.Index == hero.Battler.Index)
                         .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                         .Subscribe(x => Animation.AnimateDamage(x.Item1.gameObject));
        }

        private IObservable<EnemyController> GetSwingingLoop(EnemyController enemy)
        {
            return Observable.Return(0L)
                             .Concat(Observable.Interval(TimeSpan.FromMilliseconds(250)))
                             .Select(x => enemy);
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
                }

                BattleTargets.Clear();
                OnBattle.OnNext(Unit.Default);
            }
        }
    }
}