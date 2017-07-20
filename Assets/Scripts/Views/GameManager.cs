﻿using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public class GameManager : MonoBehaviour
    {
        // この辺りはあとでScriptableObject化
        [SerializeField]
        private int childBoundMinSize = 6;
        [SerializeField]
        private int parentBoundMinSize = 12;
        [SerializeField]
        private int marginSize = 4;
        [SerializeField]
        private int pathThickness = 2;
        [SerializeField]
        private int worldWidth = 640;
        [SerializeField]
        private int worldHeight = 480;
        [SerializeField]
        private int roomMinSize = 24;
        [SerializeField]
        private int pathReducingChance = 4;

        [SerializeField]
        private new Camera camera;

        private GameObject player;

        private void Start()
        {
            // この辺りはあとでModelへ移動
            var generator = new MapGenerator()
            {
                ChildBoundMinSize = childBoundMinSize,
                ParentBoundMinSize = parentBoundMinSize,
                MarginSize = marginSize,
                PathThickness = pathThickness,
                RoomMinSize = roomMinSize,
                PathReducingChance = pathReducingChance,
            };
            var leftBottom = new Vector2(-worldWidth / 2, -worldHeight / 2);
            var rightTop = new Vector2(worldWidth / 2, worldHeight / 2);
            var map = generator.GenerateMap(leftBottom, rightTop);

            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var mapObj = Instantiate(mapPrefab);
            mapObj.Initialize(camera, map);

            var playerPrefab = Resources.Load<GameObject>("Prefabs/Character/Player");
            player = Instantiate(playerPrefab);
            player.transform.position = map.Divisions[0].Room.Position
                + map.Divisions[0].Room.Size / 2;
            player.transform.SetPositionZ(-2);
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

            camera.transform.position = player.transform.position.ZReplacedBy(-10);
        }
    }
}