using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour, IAdventureView
    {
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        public Script_SpriteStudio_ManagerDraw managerDraw;
        [SerializeField]
        private GameObject gameUi;

        private GameEventFacade EventFacade { get; set; }
        private HeroController HeroController { get; set; }
        private EnemyController[] EnemyControllers { get; set; }
        public IObservable<Models.Enemy> BattleObservable { get; private set; }
        public BattlerController[] Battlers
        {
			get
			{
				return EnemyControllers.Cast<BattlerController>()
									   .Append(HeroController)
									   .ToArray();
            }
        }

        private void Start()
		{
			EventFacade = new GameEventFacade();
			BattleObservable = EventFacade.OnPlayerBattleWithEnemyReceiver;

            var asset = Resources.Load<DungeonGenAsset>("Assets/DungeonGenAsset");
            var model = new Models.GameManager();
			model.Initialize(asset, this);
        }

		public void Initialize(AdventureContext context)
        {
            SetMapUp(context);
            SetHeroUp(context);
            SetMaptipUp(context);
            SetEnemiesUp(context);
        }

        private void SetEnemiesUp(AdventureContext context)
        {
            var enemyPrefab = Resources.Load<EnemyController>("Prefabs/Character/Enemy_Control");
            var list = new List<EnemyController>();
            foreach (var enemy in context.Enemeis)
            {
                var obj = Instantiate(enemyPrefab);
                obj.transform.position = enemy.InitialPosition.ToVector3()
                    .MergeZ(enemyPrefab.transform.position.z);
                obj.Initialize(enemy);
                obj.transform.SetParent(managerDraw.transform);
                list.Add(obj);
            }
            EnemyControllers = list.ToArray();
        }

        private static void SetMaptipUp(AdventureContext context)
        {
            var maptipManagerPrefab = Resources.Load<MapTipRenderer>("Prefabs/Manager/MaptipRenderer");
            var maptipManager = Instantiate(maptipManagerPrefab);
            maptipManager.Initialize(context.Map);
        }

        private void SetMapUp(AdventureContext context)
        {
            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var mapObj = Instantiate(mapPrefab);
            mapObj.Initialize(context.Map, this);
        }

        private void SetHeroUp(AdventureContext context)
        {
			var heroPrefab = Resources.Load<HeroController>("Prefabs/Character/Hero");
            HeroController = Instantiate(heroPrefab);
            HeroController.transform.position = context.Map.StartLocation;
            HeroController.transform.SetPositionZ(heroPrefab.transform.position.z);
			HeroController.Initialize(context.Hero, EventFacade, EventFacade);
            HeroController.transform.SetParent(managerDraw.transform);
        }

        // Update is called once per frame
        void Update()
        {
            if (HeroController != null)
			{
				camera.transform.position = HeroController.transform.position.MergeZ(-10);
            }
        }

        public void Knockback(Battler battlerSubject, Battler battlerAgainst, int power)
        {
            var battlers = Battlers;
            var subject = battlers.FirstOrDefault(x => x.Battler.Index == battlerSubject.Index);
            var against = battlers.FirstOrDefault(x => x.Battler.Index == battlerAgainst.Index);

            if (subject != null && against != null)
            {
                subject.Knockback(against, power);
            }
        }

        public void ShowDeath(Battler subject)
        {
            var obj = Battlers.FirstOrDefault(x => x.Battler.Index == subject.Index);
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}