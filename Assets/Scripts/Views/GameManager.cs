using System.Collections;
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

        private Player player;

        private void Start()
        {
            var model = new Models.GameManager();
            var map = model.GenerateMap();

            var mapPrefab = Resources.Load<MapView>("Prefabs/Dungeon/Map");
            var mapObj = Instantiate(mapPrefab);
            mapObj.Initialize(map, this);

            var playerPrefab = Resources.Load<Player>("Prefabs/Character/Player_Control");
            player = Instantiate(playerPrefab);
            player.transform.position = map.Divisions[0].Room.Position
                + map.Divisions[0].Room.Size / 2;
            player.transform.SetPositionZ(-2);
            player.transform.SetParent(managerDraw.transform);
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

            camera.transform.position = player.transform.position.MergeZ(-10);
        }
    }
}