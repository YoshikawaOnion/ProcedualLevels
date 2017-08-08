using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour, IAdventureView
	{
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
        }

        public void Initialize(AdventureContext context)
		{
			SetHeroUp(context);

			Context = new AdventureViewContext
			{
				Hero = HeroController,
				EventReceiver = EventFacade,
				Model = context,
                Manager = this
			};

            SetMaptipUp(context);
			SetEnemiesUp(context, Context);
			SetMapUp(context, Context);
            RootObjectRepository.I.GameUi.SetActive(true);

            var timeLimit = RootObjectRepository.I.GameUi.transform.Find("TimeLimit")
                                                .GetComponent<TimeLimit>();
            timeLimit.Initialize(context);
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
            MapView.Initialize(context.Map, this, viewContext);
        }

        private void SetHeroUp(AdventureContext context)
        {
            var heroPrefab = Resources.Load<HeroController>("Prefabs/Character/Hero");
            HeroController = Instantiate(heroPrefab);
            HeroController.transform.position = context.Map.StartLocation + Vector2.one * 0.5f;
            HeroController.transform.SetPositionZ(heroPrefab.transform.position.z);
            HeroController.Initialize(context.Hero, RootObjectRepository.I.GameUi, EventFacade);
            HeroController.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
        }


        void Update()
        {
            if (HeroController != null)
            {
                RootObjectRepository.I.Camera.transform.position =
                                        HeroController.transform.position.MergeZ(-10);
            }
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

        public IObservable<IAdventureView> ResetAsync()
		{
			var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
			var view = Instantiate(viewPrefab);
			Destroy(gameObject);
            RootObjectRepository.I.GameUi.transform.Find("ClearText").gameObject.SetActive(false);
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