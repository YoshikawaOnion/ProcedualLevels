using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 敵キャラクターのアニメーションを管理するクラス。
    /// </summary>
    public class EnemyAnimationController : MonoBehaviour
	{
		[SerializeField]
		private string IdleKey = "Idle_1";
		[SerializeField]
		private string AttackKey = "Attack_1";
		[SerializeField]
        private string DamageKey = "Damage_1";

        public static readonly int LoopInfinite = 0;

        private Script_SpriteStudio_Root Sprite { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private bool IsDead { get; set; }

        /// <summary>
        /// 敵キャラクターのアニメーションの管理を開始します。
        /// </summary>
        /// <param name="eventReceiver">ゲームイベント受信口。</param>
        public void Initialize(IGameEventReceiver eventReceiver)
        {
            IsDead = false;

            Sprite = GetComponentInChildren<Script_SpriteStudio_Root>();
            Disposable = new CompositeDisposable();
            PlayAnimation(IdleKey, LoopInfinite);

            // 戦闘の参加者が自分だったら攻撃モーションを再生
            var enemy = GetComponent<EnemyController>();
            var responsibleBattle = eventReceiver.OnPlayerBattleWithEnemyReceiver
                                                 .Where(x => x.Index == enemy.Enemy.Index)
                                                 .Where(x => !IsDead);
            responsibleBattle.Subscribe(x => PlayAnimation(AttackKey, 1))
                             .AddTo(Disposable);

            // 攻撃モーションが終わると待機モーションを再生
            responsibleBattle.SelectMany(WaitAnimationFinish())
                             .Repeat()
                             .Subscribe(x => PlayAnimation(IdleKey, LoopInfinite))
                             .AddTo(Disposable);
        }

        /// <summary>
        /// 死亡時のアニメーションを再生します。
        /// </summary>
        /// <returns>アニメーション終了時に値が発行されるストリーム。</returns>
        public IObservable<Unit> AnimateDie()
        {
            IsDead = true;
            if (Disposable != null)
			{
				Disposable.Dispose();
				Disposable = null;
            }

            PlayAnimation(DamageKey, 1);

            return WaitAnimationFinish();
        }

        private IObservable<Unit> WaitAnimationFinish()
        {
            return this.UpdateAsObservable()
		               .SkipWhile(x => Sprite.AnimationCheckPlay())
		               .Select(x => Unit.Default)
		               .FirstOrDefault();
        }

        private void PlayAnimation(string key, int loopTimes)
        {
            var index = Sprite.IndexGetAnimation(key);
            Sprite.AnimationPlay(index, loopTimes);
        }
    }
}