﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedualLevels.Models;
using ProcedualLevels.Common;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;

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
        private HeroMoveController JumpController { get; set; }

        private ControllerButton LeftButton { get; set; }
        private ControllerButton RightButton { get; set; }
        private ControllerButton JumpButton { get; set; }
        private CompositeDisposable HeroDisposable { get; set; }

        /// <summary>
        /// プレイヤーキャラクターの制御を開始します。
        /// </summary>
        /// <param name="hero">プレイヤーキャラクターのモデル クラス。</param>
        /// <param name="eventAccepter">このクラスからのイベントを受け付けるインスタンス。</param>
        public void Initialize(Hero hero,
                               GameObject gameUi,
                               IPlayerEventAccepter eventAccepter)
		{
			base.Initialize(hero);
            Hero = hero;
            EventAccepter = eventAccepter;
            JumpController = GetComponent<HeroMoveController>();
            HeroDisposable = new CompositeDisposable();

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
                Animation.AnimateAttack(x.gameObject);
            })
                .AddTo(HeroDisposable);

            LeftButton = gameUi.transform.Find("Controller/LeftButton")
                               .GetComponent<ControllerButton>();
            RightButton = gameUi.transform.Find("Controller/RightButton")
                                .GetComponent<ControllerButton>();
            JumpButton = gameUi.transform.Find("Controller/JumpButton")
                               .GetComponent<ControllerButton>();
        }
		
		public override void Control()
		{
            var horizontalInput = (int)Input.GetAxisRaw("Horizontal");
            var move = horizontalInput
                + (LeftButton.IsHold ? -1 : 0)
                + (RightButton.IsHold ? 1 : 0);
            JumpController.ControlWalk(move);

            var jump = Input.GetKey(KeyCode.Space)
                            || JumpButton.IsHold;
			JumpController.ControlJump(jump);
        }

        public override void Die()
        {
            HeroDisposable.Dispose();
            JumpController.InitializeState();
            Animation.AnimateDie()
                     .Subscribe(x =>
            {
                Destroy(gameObject);
            });
            //*
            EventAccepter.OnPlayerDieSender
                         .OnNext(Unit.Default);
            //*/
        }
    }
}