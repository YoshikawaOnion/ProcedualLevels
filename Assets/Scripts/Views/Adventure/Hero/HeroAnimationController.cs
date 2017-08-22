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
        public static readonly int DiePriority = 10;

        [SerializeField]
        private Script_SpriteStudio_Root Sprite;

        private int direction_;
        private int Direction
        {
            get { return direction_; }
            set
            {
                if (value == 0)
                {
                    throw new InvalidOperationException("Direction は 0 にできません。");
                }
                direction_ = value;
            }
        }
        private int CurrentPriority { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private IObservable<int> DirectionSubject { get; set; }
        private bool IsDead { get; set; }
        private string AnimationKeyPlaying { get; set; }
        private CompositeDisposable PlayingDisposable { get; set; }

        private Dictionary<string, HeroAnimationSetting> Settings { get; set; }

        /// <summary>
        /// アニメーションの管理を開始します。
        /// </summary>
		public void Initialize(IObservable<int> directionStream)
		{
            Disposable = new CompositeDisposable();
            DirectionSubject = directionStream;
            IsDead = false;
            Direction = 1;
            CurrentPriority = 0;

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

        public void AnimateNeutral(int priority = 0, int overwritePriority = 0)
        {
            InitializeState();

            if (Direction == 0)
            {
                PlayAnimation(Settings["Idle"], Direction, priority, overwritePriority);
            }
            else
            {
                PlayAnimation(Settings["Walk"], Direction, priority, overwritePriority);
            }

            DirectionSubject.DistinctUntilChanged()
                            .Subscribe(direction =>
            {
                if (direction == 0)
                {
                    PlayAnimation(Settings["Idle"], Direction, priority, overwritePriority);
                }
                else
                {
                    PlayAnimation(Settings["Walk"], (int)direction, priority, overwritePriority);
                    Direction = Helper.Sign(direction);
                }                
            }).AddTo(PlayingDisposable);
        }

        /// <summary>
        /// 敵に攻撃する際のアニメーションを再生します。
        /// </summary>
        public void AnimateAttack(GameObject target, int priority = 0, int overwritePriority = 0)
        {
            if (IsDead)
            {
                return;
            }

            InitializeState();

            var direction = target.transform.position - transform.position;
            Direction = Helper.Sign(direction.x);
            PlayAnimation(Settings["Attack"], Direction, priority, overwritePriority);
        }

        /// <summary>
        /// 敵から攻撃を受けた際のアニメーションを再生します。
        /// </summary>
        /// <param name="target">Target.</param>
		public void AnimateDamage(GameObject target, int priority = 0, int overwritePriority = 0)
		{
			if (IsDead)
			{
				return;
			}

            InitializeState();

            var direction = target.transform.position - transform.position;
            Direction = (int)Mathf.Sign(direction.x);
            PlayAnimation(Settings["Damage"], Direction, priority, overwritePriority);

			WaitAnimationFinish().Subscribe(x => PlayAnimation(Settings["Damage"], Direction, priority, overwritePriority))
                                 .AddTo(PlayingDisposable);
		}

        public void AnimateGrabingWall(int direction, int priority = 0, int overwritePriority = 0)
        {
            if (IsDead)
            {
                return;
            }

            InitializeState();

            Direction = Helper.Sign(direction);
            PlayAnimation(Settings["GrabingWall"], Direction, priority, overwritePriority);
        }

        public void AnimateWallJump(int direction, int priority = 0, int overwritePriority = 0)
        {
            if (IsDead)
            {
                return;
            }

            InitializeState();

            Direction = Helper.Sign(direction);
            PlayAnimation(Settings["WallJump"], Direction, priority, overwritePriority);
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

            PlayAnimation(Settings["Damage"], Direction, DiePriority, DiePriority);
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

        private void PlayAnimation(HeroAnimationSetting setting, int direction, int priority, int overwritePriority)
        {
            if (CurrentPriority > priority)
            {
                return;
            }
            CurrentPriority = overwritePriority;

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
