using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedualLevels.Models;
using ProcedualLevels.Common;
using UniRx;
using UniRx.Triggers;
using System;

namespace ProcedualLevels.Views
{
    public class HeroController : BattlerController
    {
        [SerializeField]
        private float walkSpeed;

        public Hero Hero { get; private set; }
        private IPlayerEventAccepter EventAccepter { get; set; }
        private HeroAnimationController Animation { get; set; }

        public void Initialize(Hero hero,
                               IPlayerEventAccepter eventAccepter)
		{
			base.Initialize(hero);
            Hero = hero;
            EventAccepter = eventAccepter;

            Animation = GetComponent<HeroAnimationController>();
            Animation.Initialize();

            // 敵とぶつかるとバトルのメッセージを流す。ただし一度バトルしたら一定時間はぶつかってもバトルしない
            this.OnCollisionStay2DAsObservable()
                .Select(x => x.gameObject.GetComponent<EnemyController>())
                .Where(x => x != null)
                .ThrottleFirst(TimeSpan.FromMilliseconds(750))
                .Subscribe(x =>
			{
				EventAccepter.OnPlayerBattleWithEnemySender
							 .OnNext(x.Enemy);
                Animation.AnimateAttack();
            });
        }
		
		public override void Control()
		{
			float velocity = 0;
			if (Input.GetKey(KeyCode.RightArrow))
			{
				velocity += walkSpeed;
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				velocity -= walkSpeed;
			}
			Rigidbody.velocity = Rigidbody.velocity.MergeX(velocity);

            Animation.AnimateWalk(velocity);
		}

        public override void Die()
        {
            this.enabled = false;
            Animation.AnimateDie()
                     .Subscribe(x => Destroy(gameObject));
        }
    }
}