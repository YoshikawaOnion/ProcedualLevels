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
		public static readonly string IdleKey = "Idle_1";
		public static readonly string AttackKey = "Attack_1";
		public static readonly string DamageKey = "Damage_1";
        public static readonly int LoopInfinite = 0;

        private Script_SpriteStudio_Root Sprite { get; set; }
        private CompositeDisposable Disposable { get; set; }
        private bool IsDead { get; set; }

        public void Initialize(IGameEventReceiver eventReceiver)
        {
            IsDead = false;
            Sprite = transform.Find("Enemy2").GetComponent<Script_SpriteStudio_Root>();
            PlayAnimation(IdleKey, LoopInfinite);

            // 戦闘の参加者が自分だったら攻撃モーションを再生
            var enemy = GetComponent<EnemyController>();
            var responsibleBattle = eventReceiver.OnPlayerBattleWithEnemyReceiver
                                                 .Where(x => x.Index == enemy.Enemy.Index)
                                                 .Where(x => !IsDead);
            responsibleBattle.Subscribe(x => PlayAnimation(AttackKey, 1));

            // 攻撃モーションが終わると待機モーションを再生
            responsibleBattle.SelectMany(WaitAnimationFinish())
                             .Repeat()
                             .Subscribe(x => PlayAnimation(IdleKey, LoopInfinite));
        }

        /// <summary>
        /// 死亡時のアニメーションを再生します。
        /// </summary>
        /// <returns>アニメーション終了時に値が発行されるストリーム。</returns>
        public IObservable<Unit> AnimateDie()
        {
            IsDead = true;
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