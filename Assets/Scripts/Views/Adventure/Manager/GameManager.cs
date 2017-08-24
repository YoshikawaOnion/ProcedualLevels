using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// 冒険画面のビュー機能の実装をモデルに提供するクラス。
    /// </summary>
    public class GameManager : MonoBehaviour, IAdventureView
    {
        [SerializeField]
        private GameUiManager gameUiPrefab = null;

        private AdventureViewContext Context { get; set; }
        private GameEventFacade EventFacade { get; set; }
        private GameObjectManager ObjectManager { get; set; }
        private bool IsQuiting { get; set; }

        /// <summary>
        /// プレイヤーが敵と戦闘したことを通知するストリームを取得します。
        /// </summary>
        public IObservable<Enemy> OnBattle { get; private set; }
        /// <summary>
        /// プレイヤーがパワーアップアイテムを入手したことを通知するストリームを取得します。
        /// </summary>
        public IObservable<PowerUp> OnGetPowerUp { get; private set; }
        /// <summary>
        /// プレイヤーがゴールへ到達したことを通知するストリームを取得します。
        /// </summary>
        public IObservable<Unit> OnGoal { get; private set; }
        /// <summary>
        /// プレイヤーの死亡処理をしたことを通知するストリームを取得します。
        /// </summary>
        public IObservable<Unit> OnPlayerDie { get; private set; }
        /// <summary>
        /// プレイヤーが敵から一方的に攻撃されたことを通知するストリームを取得します。
        /// </summary>
        public IObservable<Enemy> OnAttacked { get; private set; }
        /// <summary>
        /// プレイヤーがトゲにぶつかったことを通知するストリームを取得します。
        /// </summary>
        public IObservable<Tuple<Spike, Battler>> OnBattlerTouchSpike { get; private set; }

        private void Start()
        {
            EventFacade = new GameEventFacade();
            OnBattle = EventFacade.OnPlayerBattleWithEnemyReceiver;
            OnGetPowerUp = EventFacade.OnPlayerGetPowerUpReceiver;
            OnGoal = EventFacade.OnPlayerGoalReceiver;
            OnPlayerDie = EventFacade.OnPlayerDieReceiver;
            OnAttacked = EventFacade.OnPlayerAttackedByEnemyReceiver;
            OnBattlerTouchSpike = EventFacade.OnBattlerTouchedSpikeReceiver
                                             .Select(x => Tuple.Create(x.Item1.Spike, x.Item2.Battler));
            IsQuiting = false;

            var gomPrefab = Resources.Load<GameObjectManager>("Prefabs/Manager/GameObjectManager");
            ObjectManager = Instantiate(gomPrefab);
        }

        private void OnApplicationQuit()
        {
            IsQuiting = true;
        }

        private void OnDestroy()
        {
            if (IsQuiting)
            {
                return;
            }
            Destroy(Context.UiManager.gameObject);
            Destroy(ObjectManager.gameObject);
        }

        /// <summary>
        /// このビューを初期化します。
        /// </summary>
        /// <param name="context">モデル側のコンテキスト オブジェクト。</param>
        public void Initialize(AdventureContext modelContext)
        {
            var gameUi = Instantiate(gameUiPrefab);
            this.UpdateAsObservable()
                .Subscribe(x =>
            {
                if (Context.Hero != null)
                {
                    var heroPos = Context.Hero.transform.position.MergeZ(-10);
                    RootObjectRepository.I.Camera.transform.position = heroPos.AddY(1);
                }
            });

            Context = new AdventureViewContext
            {
                EventReceiver = EventFacade,
                Model = modelContext,
                UiManager = gameUi,
                ObjectManager = ObjectManager
            };

            ObjectManager.Initialize(modelContext, Context, EventFacade);
            Context.Hero = ObjectManager.HeroController;

            gameUi.TimeLimitLabel.Initialize(modelContext);
            gameUi.ClearText.SetActive(false);
            modelContext.Score.Subscribe(x => gameUi.Score.text = x.ToString());
        }

        /// <summary>
        /// ノックバック情報に基づいてノックバックを発生させます。
        /// </summary>
        /// <param name="info">ノックバック情報</param>
        public void Knockback(KnockbackInfo info)
        {
            var battlers = ObjectManager.Battlers;
            var subject = battlers.FirstOrDefault(x => x.Battler.Index == info.BattlerSubject.Index);
            var against = battlers.FirstOrDefault(x => x.Battler.Index == info.BattlerAgainst.Index);

            if (subject != null && against != null)
            {
                subject.Knockback(info, against);
            }
        }

        /// <summary>
        /// キャラクターの死亡をビューに反映します。
        /// </summary>
        /// <param name="subject">死亡したキャラクターのモデル。</param>
        public void ShowDeath(Battler subject)
        {
            var obj = ObjectManager.Battlers.FirstOrDefault(x => x.Battler.Index == subject.Index);
            obj.Die();
        }

        /// <summary>
        /// パワーアップアイテムを配置します。
        /// </summary>
        /// <param name="index">パワーアップアイテムを落とした敵のインデックス。</param>
        /// <param name="powerUp">パワーアップアイテムのモデル。</param>
        public void PlacePowerUp(int battlerIndex, PowerUp powerUp)
        {
            ObjectManager.SpawnPowerUp(battlerIndex, powerUp);
        }

        /// <summary>
        /// ゲームをリセットし、新しいゲームを管理するビューを返します。
        /// </summary>
        /// <returns>新しいゲームのビュー。</returns>
        public IObservable<IAdventureView> ResetAsync()
        {
            Destroy(gameObject);
            Time.timeScale = 1;

            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            IAdventureView view = Instantiate(viewPrefab);
            return Observable.NextFrame()
                             .Select(x => view);
        }

        public IObservable<IResultView> GotoResult(int restTime, int score)
        {
            Destroy(gameObject);
            Time.timeScale = 1;

            var viewPrefab = Resources.Load<Views.ResultUiController>("Prefabs/UI/ResultUi");
            var view = Instantiate(viewPrefab);
            view.Initialize(restTime, score);
            return Observable.NextFrame()
                             .Select(x => (IResultView)view);
        }

        public void StopGame()
        {
            Time.timeScale = 0;
        }

        public IObservable<IGameOverView> GotoGameOver()
        {
            Destroy(gameObject);
            Time.timeScale = 1;

            var viewPrefab = Resources.Load<GameOverController>("Prefabs/UI/GameOverUi");
            var view = Instantiate(viewPrefab);
            view.Initialize();

            return Observable.NextFrame()
                             .Select(x => (IGameOverView)view);
        }
    }
}