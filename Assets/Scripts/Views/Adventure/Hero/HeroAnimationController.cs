﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class HeroAnimationSetting
    {
        public string LeftAnimationKey { get; private set; }
        public string RightAnimationKey { get; private set; }
        public bool IsOneShot { get; private set; }

        public HeroAnimationSetting(string leftAnimationKey, string rightAnimationKey, bool isOneShot)
        {
            IsOneShot = isOneShot;
            RightAnimationKey = rightAnimationKey;
            LeftAnimationKey = leftAnimationKey;
        }
    }

    /// <summary>
    /// プレイヤーキャラクターのアニメーションを管理するクラス。
    /// </summary>
    public class HeroAnimationController : MonoBehaviour
	{
		public static readonly string IdleLeftAnimation = "Idle_Left";
		public static readonly string IdleRightAnimation = "Idle_Right";
		public static readonly string WalkLeftAnimation = "Walk_Left";
		public static readonly string WalkRightAnimation = "Walk_Right";
		public static readonly string JumpLeftAnimation = "Jump_Left";
		public static readonly string JumpRightAnimation = "Jump_Right";
		public static readonly string AttackLeftAnimation = "Attack_Left";
		public static readonly string AttackRightAnimation = "Attack_Right";
		public static readonly string DamageLeftAnimation = "Damage_Left";
		public static readonly string DamageRightAnimation = "Damage_Right";
        public static readonly string GrabingWallLeftAnimation = "Kabe_Left";
        public static readonly string GrabingWallRightAnimation = "Kabe_Right";
        public static readonly string WallJumpLeftAnimation = "Roll_Left2";
        public static readonly string WallJumpRightAnimation = "Roll_Right2";
        public static readonly int LoopInfinite = 0;

        [SerializeField]
        private Script_SpriteStudio_Root Sprite;

        private int Direction { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private Subject<float> DirectionSubject { get; set; }
        private bool IsDead { get; set; }
        private string AnimationKeyPlaying { get; set; }
        private CompositeDisposable PlayingDisposable { get; set; }

        private Dictionary<string, HeroAnimationSetting> Settings { get; set; }

        /// <summary>
        /// アニメーションの管理を開始します。
        /// </summary>
		public void Initialize()
		{
            Disposable = new CompositeDisposable();
            DirectionSubject = new Subject<float>();
            IsDead = false;
            Direction = 1;

            Settings = new Dictionary<string, HeroAnimationSetting>();
            Settings["Idle"] = new HeroAnimationSetting(IdleLeftAnimation, IdleRightAnimation, false);
            Settings["Walk"] = new HeroAnimationSetting(WalkLeftAnimation, WalkRightAnimation, false);
            Settings["Jump"] = new HeroAnimationSetting(JumpLeftAnimation, JumpRightAnimation, false);
            Settings["Attack"] = new HeroAnimationSetting(AttackLeftAnimation, AttackRightAnimation, false);
            Settings["Damage"] = new HeroAnimationSetting(DamageLeftAnimation, DamageRightAnimation, true);
            Settings["GrabingWall"] = new HeroAnimationSetting(GrabingWallLeftAnimation, GrabingWallRightAnimation, false);
            Settings["WallJump"] = new HeroAnimationSetting(WallJumpLeftAnimation, WallJumpRightAnimation, false);

            AnimateNeutral();
        }

        public void InitializeState()
        {
            if (PlayingDisposable != null)
            {
                PlayingDisposable.Dispose();
            }
            PlayingDisposable = new CompositeDisposable();
        }

        public void AnimateNeutral()
        {
            InitializeState();
            DirectionSubject.DistinctUntilChanged()
                            .Subscribe(direction =>
            {
                if (Mathf.Abs(direction) < float.Epsilon)
                {
                    PlayAnimation(Settings["Idle"], Direction);
                }
                else
                {
                    PlayAnimation(Settings["Walk"], (int)direction);
                    Direction = Helper.Sign(direction);
                }                
            }).AddTo(PlayingDisposable);
        }

        /// <summary>
        /// 指定した歩行方向をアニメーションマネージャに設定します。
        /// </summary>
        /// <param name="direction">現在の歩行方向。</param>
        public void UpdateWalkDirection(float direction)
        {
            if (IsDead)
            {
                return;
            }

            DirectionSubject.OnNext(direction);
        }

        /// <summary>
        /// 敵に攻撃する際のアニメーションを再生します。
        /// </summary>
        public void AnimateAttack(GameObject target)
        {
            if (IsDead)
            {
                return;
            }

            InitializeState();

            var direction = target.transform.position - transform.position;
            Direction = Helper.Sign(direction.x);
            PlayAnimation(Settings["Attack"], Direction);
        }

        /// <summary>
        /// 敵から攻撃を受けた際のアニメーションを再生します。
        /// </summary>
        /// <param name="target">Target.</param>
		public void AnimateDamage(GameObject target)
		{
			if (IsDead)
			{
				return;
			}

            InitializeState();

            var direction = target.transform.position - transform.position;
            PlayAnimation(Settings["Damage"], (int)direction.x);
            Direction = Helper.Sign((int)direction.x);

			WaitAnimationFinish().Subscribe(x => PlayAnimation(Settings["Damage"], Direction))
                                 .AddTo(PlayingDisposable);
		}

        /// <summary>
        /// 死亡時のアニメーションを再生します。
        /// </summary>
        /// <returns>アニメーション終了時に値を発行するストリーム。</returns>
        public IObservable<Unit> AnimateDie()
        {
            IsDead = true;
            if (PlayingDisposable != null)
            {
                PlayingDisposable.Dispose();
            }

            PlayAnimation(Settings["Damage"], Direction);
            return WaitAnimationFinish();
        }

        /// <summary>
        /// 現在再生しているアニメーションが終了するのを待機します。
        /// </summary>
        /// <returns>現在再生しているアニメーションが終了すると値が発行されるストリーム。</returns>
        private IObservable<Unit> WaitAnimationFinish()
        {
            return this.UpdateAsObservable()
					   .SkipWhile(x => Sprite.AnimationCheckPlay())
                       .Select(x => Unit.Default)
					   .FirstOrDefault();
        }
		
		private void PlayAnimation(string animationKey, int playTimes)
		{
			var index = Sprite.IndexGetAnimation(animationKey);
			Sprite.AnimationPlay(index, playTimes);
            AnimationKeyPlaying = animationKey;
		}

        private void OnDestroy()
        {
            Sprite = null;
            if (Disposable != null)
			{
				Disposable.Dispose();
            }
            if (PlayingDisposable != null)
            {
                PlayingDisposable.Dispose();
            }
        }

        private void PlayAnimation(HeroAnimationSetting setting, int direction)
        {
            var loopTimes = setting.IsOneShot ? 1 : LoopInfinite;
            if (direction > 0)
            {
                PlayAnimation(setting.RightAnimationKey, loopTimes);
            }
            else if (direction < 0)
            {
                PlayAnimation(setting.LeftAnimationKey, loopTimes);
            }
        }
    }
}
