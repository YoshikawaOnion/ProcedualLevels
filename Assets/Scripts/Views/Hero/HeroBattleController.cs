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
    public class HeroBattleController : MonoBehaviour
    {
        private List<EnemyController> BattleTargets { get; set; }
        private IPlayerEventAccepter EventAccepter { get; set; }
		private CompositeDisposable Disposable { get; set; }
		private HeroAnimationController Animation { get; set; }
		private HeroMoveController Move { get; set; }
        private Subject<Unit> OnBattle { get; set; }

        public void Initialize(IPlayerEventAccepter eventAccepter)
        {
            BattleTargets = new List<EnemyController>();
            EventAccepter = eventAccepter;
            Disposable = new CompositeDisposable();
            Animation = GetComponent<HeroAnimationController>();
            Move = GetComponent<HeroMoveController>();
            OnBattle = new Subject<Unit>();

            gameObject.OnCollisionStay2DAsObservable()
                      .Select(x => x.gameObject.GetComponent<EnemyController>())
                      .Where(x => x != null)
                      .PauseBy(OnBattle, TimeSpan.FromMilliseconds(750))
                      .Subscribe(x => BattleTargets.Add(x))
                      .AddTo(Disposable);
        }

        void Update()
        {
            if (BattleTargets.Any())
            {
                foreach (var target in BattleTargets)
				{
					var distance = target.transform.position - transform.position;
					if (distance.x * Move.WalkDirection > 0)
					{
						EventAccepter.OnPlayerBattleWithEnemySender
									 .OnNext(target.Enemy);
						Animation.AnimateAttack(target.gameObject);
					}
					else
					{
						EventAccepter.OnPlayerAttackedByEnemySender
									 .OnNext(target.Enemy);
						//Animation.AnimateDamage(target.gameObject);
					}
                }

                BattleTargets.Clear();
                OnBattle.OnNext(Unit.Default);
            }
        }
    }
}