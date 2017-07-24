using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour, IAdventureView
    {
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        public Script_SpriteStudio_ManagerDraw managerDraw;

        private Player Player { get; set; }
        private GameEventFacade EventFacade { get; set; }
        private HeroController HeroController { get; set; }

        private void Start()
		{
            var asset = Resources.Load<DungeonGenAsset>("Assets/DungeonGenAsset");
            var model = new Models.GameManager();
			model.Initialize(asset, this);
        }

		public void Initialize(AdventureContext context)
        {
            EventFacade = new GameEventFacade();
            SetMapUp(context);
            SetHeroUp(context);
            SetMaptipUp(context);
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