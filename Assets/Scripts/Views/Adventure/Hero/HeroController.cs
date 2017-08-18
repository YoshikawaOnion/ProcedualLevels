using System.Collections;
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
                               IPlayerEventAccepter eventAccepter,
                               IGameEventReceiver eventReceiver,
                               AdventureViewContext context)
        {
            base.Initialize(hero);
            Hero = hero;
            EventAccepter = eventAccepter;
            JumpController = GetComponent<HeroMoveController>();
            HeroDisposable = new CompositeDisposable();

            Animation = GetComponent<HeroAnimationController>();
            Animation.Initialize();

            LeftButton = context.UiManager.LeftButton;
            RightButton = context.UiManager.RightButton;
            JumpButton = context.UiManager.JumpButton;

            var battle = GetComponent<HeroBattleController>();
            battle.Initialize(eventAccepter, eventReceiver);

            this.OnCollisionStay2DAsObservable()
                .Subscribe(x =>
            {
                var enemy = x.gameObject.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    var contact = x.contacts[0];
                    if (contact.normal.y > 0)
                    {
                        var force = AssetRepository.I.GameParameterAsset.TrampleForce;
                        Rigidbody.AddForce(new Vector2(0, -force));
                        enemy.KnockbackState.OnTrampled();
                    }
                }
            })
                .AddTo(HeroDisposable);
        }

        private void Update()
        {
            var horizontalInput = (int)Input.GetAxisRaw("Horizontal");
            var move = horizontalInput
                + (LeftButton.IsHold ? -1 : 0)
                + (RightButton.IsHold ? 1 : 0);
            Animation.UpdateWalkDirection(move);
            JumpController.UpdateWalkDirection(move);
        }

        /// <summary>
        /// 毎フレーム行う処理を実行します。
        /// </summary>
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

        /// <summary>
        /// キャラクターが死亡した際の処理を行います。
        /// </summary>
        public override void Die()
        {
            HeroDisposable.Dispose();
            JumpController.InitializeState();
            Animation.AnimateDie()
                     .Subscribe(x =>
            {
                Destroy(gameObject);
            });
            EventAccepter.OnPlayerDieSender
                         .OnNext(Unit.Default);
        }
    }
}