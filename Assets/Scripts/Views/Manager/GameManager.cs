using System;
using System.Collections;
using System.Collections.Generic;
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

        private Player Player { get; set; }
        private GameEventFacade EventFacade { get; set; }
        private HeroController HeroController { get; set; }

        public IObservable<Models.Enemy> BattleObservable { get; private set; }

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
            foreach (var enemy in context.Enemeis)
            {
                var obj = Instantiate(enemyPrefab);
                obj.transform.position = enemy.InitialPosition.ToVector3()
                    .MergeZ(enemyPrefab.transform.position.z);
                obj.Initialize(enemy);
                obj.transform.SetParent(managerDraw.transform);
            }
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
			HeroController.Initialize(context.Hero, EventFacade);
        }

        private void SetPlayerUp(MapData map)
        {
            var playerPrefab = Resources.Load<Player>("Prefabs/Character/Player_Control");
            Player = Instantiate(playerPrefab);
            Player.transform.position = map.Divisions[0].Room.Position
                + map.Divisions[0].Room.Size / 2;
            Player.transform.SetPositionZ(-2);
            Player.transform.SetParent(managerDraw.transform);
            Player.Initialize(EventFacade, EventFacade);
        }

        // Update is called once per frame
        void Update()
        {
            camera.transform.position = HeroController.transform.position.MergeZ(-10);
        }

    }
}