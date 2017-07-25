using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    public class EnemyAnimationController : MonoBehaviour
    {
		public static readonly string IdleKey = "Idle_1";
		public static readonly string AttackKey = "Attack_1";
		public static readonly string DamageKey = "Damage_1";
        public static readonly int LoopInfinite = 0;

        private Script_SpriteStudio_Root Sprite { get; set; }
        private CompositeDisposable Disposable { get; set; }

        public void Initialize(IGameEventReceiver eventReceiver)
        {
            Sprite = transform.Find("Enemy2").GetComponent<Script_SpriteStudio_Root>();
            PlayAnimation(IdleKey, LoopInfinite);

            // 戦闘の参加者が自分だったら攻撃モーションを再生
            var enemy = GetComponent<EnemyController>();
            var responsibleBattle = eventReceiver.OnPlayerBattleWithEnemyReceiver
                                                 .Where(x => x.Index == enemy.Enemy.Index);
            responsibleBattle.Subscribe(x => PlayAnimation(AttackKey, 1));

            // 攻撃モーションが終わると待機モーションを再生
            var waitAnimationPlay = this.UpdateAsObservable()
                                        .SkipWhile(x => Sprite.AnimationCheckPlay())
                                        .Select(x => Unit.Default);
            responsibleBattle.SelectMany(waitAnimationPlay)
                             .FirstOrDefault()
                             .Repeat()
                             .Subscribe(x => PlayAnimation(IdleKey, LoopInfinite));
        }

        private void PlayAnimation(string key, int loopTimes)
        {
            var index = Sprite.IndexGetAnimation(key);
            Sprite.AnimationPlay(index, loopTimes);
        }
    }
}