using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
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
        public static readonly int LoopAnimation = 0;

        private int Direction { get; set; }
        private Script_SpriteStudio_Root Sprite { get; set; }
        private CompositeDisposable Disposable { get; set; }

		public void Initialize(IObservable<float> directionStream,
							   IGameEventReceiver eventReceiver)
		{
            Disposable = new CompositeDisposable();
            Sprite = transform.Find("Player2").GetComponent<Script_SpriteStudio_Root>();

            // 移動の状況に応じて左右のアニメーションを設定
            // プレイヤーの向きもここで更新
			directionStream.DistinctUntilChanged()
						   .Subscribe(x =>
			{
				if (x < float.Epsilon && x > -float.Epsilon)
				{
					if (Direction > 0)
					{
						PlayAnimation(IdleRightAnimation, LoopAnimation);
					}
					else
					{
						PlayAnimation(IdleLeftAnimation, LoopAnimation);
					}
				}
				else if (x > 0)
				{
					PlayAnimation(WalkRightAnimation, LoopAnimation);
					Direction = 1;
				}
				else if (x < 0)
				{
					PlayAnimation(WalkLeftAnimation, LoopAnimation);
					Direction = -1;
				}
			})
                           .AddTo(Disposable);

            // 敵とぶつかったら攻撃モーション
            eventReceiver.OnPlayerBattleWithEnemyReceiver
                         .Subscribe(x =>
            {
				if (Direction > 0)
				{
					PlayAnimation(AttackRightAnimation, 1);
				}
				else
				{
					PlayAnimation(AttackLeftAnimation, 1);
				}
            })
                         .AddTo(Disposable);

            // 現在のアニメーションが終了するまで待つストリーム
            var neutralizer = this.UpdateAsObservable()
                                  .SkipWhile(x => Sprite.AnimationCheckPlay())
                                  .Select(x => Unit.Default);

            // 攻撃モーションが終了したら待機モーションに戻す
            eventReceiver.OnPlayerBattleWithEnemyReceiver
                         .SelectMany(neutralizer)
                         .FirstOrDefault()
                         .Repeat()
                         .Subscribe(x =>
            {
                Debug.Log("Neutralize");
                if (Direction > 0)
                {
                    PlayAnimation(IdleRightAnimation, LoopAnimation);
                }
                else
                {
                    PlayAnimation(IdleLeftAnimation, LoopAnimation);
                }
            })
                         .AddTo(Disposable);
        }

		public void PlayAnimation(string animationKey, int playTimes)
		{
			var index = Sprite.IndexGetAnimation(animationKey);
			Sprite.AnimationPlay(index, playTimes);
		}
    }
}