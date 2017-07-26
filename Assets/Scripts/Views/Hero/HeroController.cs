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
    /// <summary>
    /// プレイヤーキャラクターの振る舞いを制御するクラス。
    /// </summary>
    public class HeroController : BattlerController
    {
        public Hero Hero { get; private set; }
        private IPlayerEventAccepter EventAccepter { get; set; }
        private HeroAnimationController Animation { get; set; }
        private HeroJumpController JumpController { get; set; }

        /// <summary>
        /// プレイヤーキャラクターの制御を開始します。
        /// </summary>
        /// <param name="hero">プレイヤーキャラクターのモデル クラス。</param>
        /// <param name="eventAccepter">このクラスからのイベントを受け付けるインスタンス。</param>
        public void Initialize(Hero hero,
                               IPlayerEventAccepter eventAccepter)
		{
			base.Initialize(hero);
            Hero = hero;
            EventAccepter = eventAccepter;
            JumpController = GetComponent<HeroJumpController>();

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
            JumpController.Walk();
		}

        public override void Die()
        {
            this.enabled = false;
            Animation.AnimateDie()
                     .Subscribe(x => Destroy(gameObject));
        }
    }
}