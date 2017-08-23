using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public interface IAdventureView
    {
        /// <summary>
        /// このビューを初期化します。
        /// </summary>
        /// <param name="context">モデル側のコンテキスト オブジェクト。</param>
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

        /// <summary>
        /// 探検画面のビューをリセットします。
        /// </summary>
		IObservable<IAdventureView> ResetAsync();
        /// <summary>
        /// リザルト画面に遷移します。
        /// </summary>
        /// <returns>リザルト画面のビューを通知するストリーム。</returns>
        IObservable<IResultView> GotoResult(int restTime, int score);

        /// <summary>
        /// プレイヤーが敵と戦闘したことを通知するストリームを取得します。
        /// </summary>
		IObservable<Enemy> OnBattle { get; }
        /// <summary>
        /// プレイヤーがパワーアップアイテムを入手したことを通知するストリームを取得します。
        /// </summary>
		IObservable<PowerUp> OnGetPowerUp { get; }
        /// <summary>
        /// プレイヤーがゴールへ到達したことを通知するストリームを取得します。
        /// </summary>
		IObservable<Unit> OnGoal { get; }
        /// <summary>
        /// プレイヤーの死亡処理をしたことを通知するストリームを取得します。
        /// </summary>
        IObservable<Unit> OnPlayerDie { get; }
        /// <summary>
        /// プレイヤーが敵から一方的に攻撃されたことを通知するストリームを取得します。
        /// </summary>
        IObservable<Enemy> OnAttacked { get; }
        /// <summary>
        /// プレイヤーがトゲにぶつかったことを通知するストリームを取得します。
        /// </summary>
        IObservable<Tuple<Spike, Battler>> OnBattlerTouchSpike { get; }
    }
}