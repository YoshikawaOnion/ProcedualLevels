using UnityEngine;
using System.Collections;
using System;
using ProcedualLevels.Common;
using ProcedualLevels.Models;
using System.Collections.Generic;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// マップに属する要素を管理するクラス。
    /// </summary>
    public class MapView : MonoBehaviour
    {
        private GameObject Maze { get; set; }
        private GameObject DebugRoot { get; set; }
        private Goal Goal { get; set; }
        private List<SpikeController> Spikes { get; set; }
        private bool Quitting { get; set; }

        /// <summary>
        /// マップに属する要素を表示します。
        /// </summary>
        /// <param name="map">この引数に渡したマップに属する要素を表示します。</param>
        /// <param name="viewContext">探索画面の情報を保持するコンテキスト オブジェクト。</param>
        /// <param name="eventFacade">このビューで使用するイベント送受信口。</param>
        public void Initialize(MapData map, AdventureViewContext viewContext, GameEventFacade eventFacade)
        {
            var mazePrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Maze_Control");
            Maze = Instantiate(mazePrefab);
            Maze.transform.SetParent(transform);
            Spikes = new List<SpikeController>();

            ShowRooms(map, Maze);
            ShowPlatforms(map, Maze);
            ShowGoal(map, viewContext, eventFacade);
            ShowSpawners(map, viewContext);
            ShowCollisionBlock(map);
            ShowSpikes(map, eventFacade);
        }

        private void ShowSpikes(MapData map, GameEventFacade eventFacade)
        {
            var prefab = Resources.Load<SpikeController>("Prefabs/Character/Spike_Control");
            foreach (var item in map.Spikes)
            {
                var obj = Instantiate(prefab);
                obj.transform.position = item.InitialPosition + Vector2.one * 0.5f;
                obj.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
                obj.Initialize(item, eventFacade);
                Spikes.Add(obj);
            }
        }

        private void ShowCollisionBlock(MapData map)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/CollisionBlock");
            var offset = Vector2.one * 0.5f;
            foreach (var item in map.CollisionBlocks)
            {
                var obj = Instantiate(prefab);
                obj.transform.position = item + offset;
                obj.transform.SetParent(transform);
            }
        }

        private void ShowSpawners(MapData map, AdventureViewContext viewContext)
        {
            var prefabs = new Dictionary<string, SpawnerController>();
            foreach (var spawner in map.Spawners)
            {
                SpawnerController prefab;
                if (!prefabs.TryGetValue(spawner.PrefabName, out prefab))
                {
                    prefab = Resources.Load<SpawnerController>("Prefabs/Spawner/" + spawner.PrefabName);
                    prefabs[spawner.PrefabName] = prefab;
                }

                var obj = Instantiate(prefab);
                obj.Initialize(spawner, viewContext);
                obj.transform.SetParent(transform);
            }
        }

        private void ShowGoal(MapData map, AdventureViewContext context, GameEventFacade eventFacade)
        {
            var goalPrefab = Resources.Load<Goal>("Prefabs/Dungeon/Goal_Control");
            Goal = Instantiate(goalPrefab);
            Goal.transform.position = map.GoalLocation + new Vector2(0.5f, 0.5f);
            Goal.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
            Goal.Initialize(context, eventFacade);
        }

        private void OnApplicationQuit()
        {
            Quitting = true;
        }

        private void OnDestroy()
        {
            if (Quitting)
            {
                return;
            }

            Destroy(Goal.gameObject);
            foreach (var item in Spikes)
            {
                Destroy(item.gameObject);
            }
        }

        private void ShowRooms(MapData map, GameObject maze)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Room_Control");
            DebugRoot = new GameObject();
            DebugRoot.name = "Debug_MazeViewer";
            DebugRoot.transform.SetParent(transform);

            foreach (var division in map.Divisions)
            {
                var obj = InstantiateRect(DebugRoot, prefab, division.Bound);
                obj.GetComponent<BoxCollider2D>().enabled = false;

                InstantiateRect(maze, prefab, division.Room);
            }

            foreach (var connection in map.Connections)
            {
                foreach (var segment in connection.Path.GetRooms())
                {
                    InstantiateRect(maze, prefab, segment);
                }
            }
        }

        private static void ShowPlatforms(MapData map, GameObject parent)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Platform_Control");
            foreach (var platform in map.Platforms)
            {
                var obj = Instantiate(prefab);
                var x = platform.Left;
                var width = platform.Right - platform.Left;
                obj.transform.SetParent(parent.transform);
                obj.transform.position = new Vector3(x, platform.Bottom, -1) + new Vector3(width, 1, 0) / 2;
                obj.transform.localScale = new Vector3(width, 1, 1);
            }
        }

        private static GameObject InstantiateRect(GameObject maze, GameObject prefab, MapRectangle room)
        {
            var obj = Instantiate(prefab);
            obj.transform.position = room.Position + room.Size / 2;
            obj.transform.localScale = room.Size - Vector2.one * 0.001f;
            obj.name = string.IsNullOrEmpty(room.Name) ? prefab.name : room.Name;
            if (maze != null)
            {
                obj.transform.parent = maze.transform;
            }
            return obj;
        }
    }
}