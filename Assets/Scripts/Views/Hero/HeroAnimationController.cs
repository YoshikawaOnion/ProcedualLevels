﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;
using System;

namespace ProcedualLevels.Views
{
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
        public static readonly int LoopInfinite = 0;

        private int Direction { get; set; }
        private Script_SpriteStudio_Root Sprite { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private Subject<float> DirectionSubject { get; set; }
        private bool IsDead { get; set; }

        /// <summary>
        /// アニメーションの管理を開始します。
        /// </summary>
		public void Initialize()
		{
            Disposable = new CompositeDisposable();
            Sprite = transform.Find("Player2").GetComponent<Script_SpriteStudio_Root>();
            DirectionSubject = new Subject<float>();
            IsDead = false;

			PlayAnimation(IdleRightAnimation, LoopInfinite);

            DirectionSubject.DistinctUntilChanged()
                            .Subscribe(direction =>
            {
                if (Mathf.Abs(direction) < float.Epsilon)
				{
					if (Direction > 0)
					{
						PlayAnimation(IdleRightAnimation, LoopInfinite);
					}
					else
					{
						PlayAnimation(IdleLeftAnimation, LoopInfinite);
					}
				}
				else if (direction > 0)
				{
					PlayAnimation(WalkRightAnimation, LoopInfinite);
					Direction = 1;
				}
				else if (direction < 0)
				{
					PlayAnimation(WalkLeftAnimation, LoopInfinite);
					Direction = -1;
				}
			});
        }
        /// <summary>
        /// 指定した歩行方向をアニメーションマネージャに設定し、対応するアニメーションを再生します。
        /// </summary>
        /// <param name="direction">現在の歩行方向。</param>
        public void AnimateWalk(float direction)
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

            var direction = target.transform.position - transform.position;
            if (direction.x > 0)
            {
                PlayAnimation(AttackRightAnimation, 1);
            }
            else
            {
                PlayAnimation(AttackLeftAnimation, 1);
            }

            WaitAnimationFinish().Subscribe(x =>
            {
                if (direction.x > 0)
                {
                    PlayAnimation(IdleRightAnimation, LoopInfinite);
                }
                else
                {
                    PlayAnimation(IdleLeftAnimation, LoopInfinite);
                }                
            })
                                 .AddTo(Disposable);
        }

        /// <summary>
        /// 死亡時のアニメーションを再生します。
        /// </summary>
        /// <returns>アニメーション終了時に値を発行するストリーム。</returns>
        public IObservable<Unit> AnimateDie()
        {
            IsDead = true;

            if (Direction > 0)
            {
                PlayAnimation(DamageRightAnimation, 1);
            }
            else
            {
                PlayAnimation(DamageLeftAnimation, 1);
            }

            return WaitAnimationFinish();
        }

        /// <summary>
        /// 現在再生しているアニメーションが終了するのを待機します。
        /// </summary>
        /// <returns>現在再生しているアニメーションが終了すると値が発行されるストリーム。</returns>
        private IObservable<Unit> WaitAnimationFinish(string debug = "")
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
		}

        private void OnDestroy()
        {
            Sprite = null;
            Disposable.Dispose();
        }
    }
}
