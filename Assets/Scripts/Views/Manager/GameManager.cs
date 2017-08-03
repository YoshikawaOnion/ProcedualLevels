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
		private HeroController HeroController { get; set; }
		private EnemyController[] EnemyControllers { get; set; }
        private MapView MapView { get; set; }
        private MapTipRenderer MapTipRenderer { get; set; }

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

        private void Start()
        {
            EventFacade = new GameEventFacade();
            OnBattle = EventFacade.OnPlayerBattleWithEnemyReceiver;
            OnGetPowerUp = EventFacade.OnPlayerGetPowerUpReceiver;
            OnGoal = EventFacade.OnPlayerGoalReceiver;
            OnPlayerDie = EventFacade.OnPlayerDieReceiver;
        }

        public void Initialize(AdventureContext context)
		{
			SetHeroUp(context);

			var viewContext = new AdventureViewContext
			{
				Hero = HeroController,
				EventReceiver = EventFacade,
				Model = context,
			};

            SetMaptipUp(context);
			SetEnemiesUp(context, viewContext);
			SetMapUp(context, viewContext);
            RootObjectRepository.I.GameUi.SetActive(true);

            var timeLimit = RootObjectRepository.I.GameUi.transform.Find("TimeLimit")
                                                .GetComponent<TimeLimit>();
            timeLimit.Initialize(context);
        }

        private void SetEnemiesUp(AdventureContext context, AdventureViewContext viewContext)
        {
            var list = new List<EnemyController>();

            foreach (var enemy in context.Enemies)
            {
                var obj = viewContext.SpawnEnemy(enemy);
                list.Add(obj);
            }

            EnemyControllers = list.ToArray();
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
            HeroController.transform.position = context.Map.StartLocation;
            HeroController.transform.SetPositionZ(heroPrefab.transform.position.z);
            HeroController.Initialize(context.Hero, RootObjectRepository.I.GameUi, EventFacade);
            HeroController.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
        }


        void Update()
        {
            if (HeroController != null)
            {
                RootObjectRepository.I.Camera.transform.position = HeroController.transform.position.AddY(2).MergeZ(-10);
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
            Destroy(MapView.gameObject);
            Destroy(MapTipRenderer.gameObject);
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
    }
}