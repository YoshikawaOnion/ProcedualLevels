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
    public class GameManager : MonoBehaviour, IAdventureView
    {
        [SerializeField]
        private GameUiManager gameUiPrefab;
        [SerializeField]
        private Camera gameUiCamera;

        private Dictionary<string, EnemyController> EnemyPrefabs { get; set; }

        private HeroController HeroController { get; set; }
        private List<EnemyController> EnemyControllers { get; set; }
        private MapView MapView { get; set; }
        private MapTipRenderer MapTipRenderer { get; set; }
        private AdventureViewContext Context { get; set; }

        public GameEventFacade EventFacade { get; set; }
        public BattlerController[] Battlers
        {
            get
            {
                return EnemyControllers.Cast<BattlerController>()
                                       .Append(HeroController)
                                       .ToArray();
            }
        }

        public IObservable<Enemy> OnBattle { get; private set; }
        public IObservable<PowerUp> OnGetPowerUp { get; private set; }
        public IObservable<Unit> OnGoal { get; private set; }
        public IObservable<Unit> OnPlayerDie { get; private set; }
        public IObservable<Enemy> OnAttacked { get; private set; }
        public IObservable<Tuple<Spike, Battler>> OnBattlerTouchSpike { get; private set; }

        private void Start()
        {
            EventFacade = new GameEventFacade();
            EnemyControllers = new List<EnemyController>();
            EnemyPrefabs = new Dictionary<string, EnemyController>();
            OnBattle = EventFacade.OnPlayerBattleWithEnemyReceiver;
            OnGetPowerUp = EventFacade.OnPlayerGetPowerUpReceiver;
            OnGoal = EventFacade.OnPlayerGoalReceiver;
            OnPlayerDie = EventFacade.OnPlayerDieReceiver;
            OnAttacked = EventFacade.OnPlayerAttackedByEnemyReceiver;
            OnBattlerTouchSpike = EventFacade.OnBattlerTouchedSpikeReceiver
                                             .Select(x => Tuple.Create(x.Item1.Spike, x.Item2.Battler));
        }

        public void Initialize(AdventureContext modelContext)
        {
            var gameUi = Instantiate(gameUiPrefab);
            this.UpdateAsObservable()
                .Subscribe(x =>
            {
                if (HeroController != null)
                {
                    var heroPos = HeroController.transform.position.MergeZ(-10);
                    RootObjectRepository.I.Camera.transform.position = heroPos.AddY(1);
                    gameUi.UiCamera.transform.position = heroPos.MergeZ(-2000);
                }
            });

            Context = new AdventureViewContext
            {
                EventReceiver = EventFacade,
                Model = modelContext,
                Manager = this,
                UiManager = gameUi
            };
            Context.Hero = SetHeroUp(modelContext, Context);

            SetMaptipUp(modelContext);
            SetEnemiesUp(modelContext, Context);
            SetMapUp(modelContext, Context);

            gameUi.TimeLimitLabel.Initialize(modelContext);
            gameUi.ClearText.SetActive(false);
        }

        private void SetEnemiesUp(AdventureContext context, AdventureViewContext viewContext)
        {
            foreach (var enemy in context.Enemies)
            {
                SpawnEnemy(enemy);
            }
        }

        private void SetMaptipUp(AdventureContext context)
        {
            var maptipManagerPrefab = Resources.Load<MapTipRenderer>("Prefabs/Manager/MaptipRenderer");
            MapTipRenderer = Instantiate(maptipManagerPrefab);
            MapTipRenderer.Initialize(context.Map);
        }

        private void SetMapUp(AdventureContext context, AdventureViewContext viewContext)
        {
            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            MapView = Instantiate(mapPrefab);
            MapView.Initialize(context.Map, viewContext);
        }

        private HeroController SetHeroUp(AdventureContext context, AdventureViewContext viewContext)
        {
            var heroPrefab = Resources.Load<HeroController>("Prefabs/Character/Hero");
            HeroController = Instantiate(heroPrefab);
            HeroController.transform.position = context.Map.StartLocation + Vector2.one * 0.5f;
            HeroController.transform.SetPositionZ(heroPrefab.transform.position.z);
            HeroController.Initialize(context.Hero, EventFacade, EventFacade, viewContext);
            HeroController.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
            return HeroController;
        }

        private void OnDestroy()
        {
            foreach (var battler in Battlers)
            {
                if (battler != null)
                {
                    Destroy(battler.gameObject);
                }
            }
            if (MapView != null)
            {
                Destroy(MapView.gameObject);
            }
            if (MapTipRenderer != null)
            {
                Destroy(MapTipRenderer.gameObject);
            }
            EventFacade = null;
        }


        public void Knockback(KnockbackInfo info)
        {
            var battlers = Battlers;
            var subject = battlers.FirstOrDefault(x => x.Battler.Index == info.BattlerSubject.Index);
            var against = battlers.FirstOrDefault(x => x.Battler.Index == info.BattlerAgainst.Index);

            if (subject != null && against != null)
            {
                subject.Knockback(info, against);
            }
        }

        public void ShowDeath(Battler subject)
        {
            var obj = Battlers.FirstOrDefault(x => x.Battler.Index == subject.Index);
            obj.Die();
        }

        public void PlacePowerUp(int battlerIndex, PowerUp powerUp)
        {
            var battler = Battlers.First(x => x.Battler.Index == battlerIndex);
            var prefab = Resources.Load<PowerUpItemController>("Prefabs/Character/PowerUp");
            var obj = Instantiate(prefab);
            obj.Initialize(powerUp, EventFacade);
            obj.transform.position = battler.transform.position.MergeZ(obj.transform.position.z);
            obj.transform.SetParent(transform);
        }

        /// <summary>
        /// ゲームをリセットし、新しいゲームを管理するビューを返します。
        /// </summary>
        /// <returns>新しいゲームのビュー。</returns>
        public IObservable<IAdventureView> ResetAsync()
        {
            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            var view = Instantiate(viewPrefab);
            Destroy(gameObject);
            Destroy(Context.UiManager.gameObject);
            return Observable.EveryUpdate()
                             .Skip(1)
                             .First()
                             .Select(x =>
            {
                return (IAdventureView)view;
            });
        }

        public EnemyController SpawnEnemy(Enemy enemy)
        {
            EnemyController prefab;
            if (!EnemyPrefabs.TryGetValue(enemy.Ability.PrefabName, out prefab))
            {
                prefab = Resources.Load<EnemyController>
                                  ("Prefabs/Enemy/" + enemy.Ability.PrefabName);
                EnemyPrefabs[enemy.Ability.PrefabName] = prefab;
            }

            var obj = Instantiate(prefab);
            obj.transform.position = enemy.InitialPosition
                .ToVector3()
                .MergeZ(prefab.transform.position.z);
            obj.Initialize(enemy, Context);
            obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);

            EnemyControllers.Add(obj);
            return obj;
        }
    }
}