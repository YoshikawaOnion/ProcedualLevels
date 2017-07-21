﻿using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private new Camera camera;
        [SerializeField]
        public Script_SpriteStudio_ManagerDraw managerDraw;

        private Player Player { get; set; }
        private GameEventFacade EventFacade { get; set; }

        private void Start()
		{
			EventFacade = new GameEventFacade();

            var model = new Models.GameManager();
            var map = model.GenerateMap();

            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var mapObj = Instantiate(mapPrefab);
            mapObj.Initialize(map, this);

            SetPlayerUp(map);

            var maptipManagerPrefab = Resources.Load<MapTipRenderer>("Prefabs/Manager/MaptipRenderer");
            var maptipManager = Instantiate(maptipManagerPrefab);
            maptipManager.Initialize(map);
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
            if (Input.GetMouseButtonDown(0))
            {
                var prefab = Resources.Load<GameObject>("Prefabs/Debug/Ball");
                var instance = Instantiate(prefab);
                var pos = camera.ScreenToWorldPoint(Input.mousePosition);
                instance.transform.position = new Vector3(pos.x, pos.y, -2);
            }

            camera.transform.position = Player.transform.position.MergeZ(-10);
        }
    }
}