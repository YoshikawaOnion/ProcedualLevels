using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public interface IAdventureView
    {
        void Initialize(AdventureContext context);
        /// <summary>
        /// ノックバック情報に基づいてノックバックを発生させます。
        /// </summary>
        /// <param name="info">ノックバック情報</param>
        void Knockback(KnockbackInfo info);
        /// <summary>
        /// キャラクターの死亡をビューに反映します。
        /// </summary>
        /// <param name="subject">死亡したキャラクターのモデル。</param>
        void ShowDeath(Battler subject);
        /// <summary>
        /// パワーアップアイテムを配置します。
        /// </summary>
        /// <param name="index">パワーアップアイテムのインデックス。</param>
        /// <param name="powerUp">パワーアップアイテムのモデル。</param>
        void PlacePowerUp(int index, PowerUp powerUp);

		IObservable<IAdventureView> ResetAsync();

		IObservable<Enemy> OnBattle { get; }
		IObservable<PowerUp> OnGetPowerUp { get; }
		IObservable<Unit> OnGoal { get; }
        IObservable<Unit> OnPlayerDie { get; }
        IObservable<Enemy> OnAttacked { get; }
        IObservable<Tuple<Spike, Battler>> OnBattlerTouchSpike { get; }
    }
}