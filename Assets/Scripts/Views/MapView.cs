using UnityEngine;
using System.Collections;
using System;
using ProcedualLevels.Common;
using ProcedualLevels.Models;

namespace ProcedualLevels.Views
{
    public class MapView : MonoBehaviour
    {
        private GameObject Maze { get; set; }
        private GameObject DebugRoot { get; set; }
        private Goal Goal { get; set; }

        public void Initialize(MapData map, GameManager manager, AdventureViewContext viewContext)
        {
            var mazePrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Maze_Control");
            Maze = Instantiate(mazePrefab);

            ShowRooms(map, Maze);
            ShowPlatforms(map, Maze);
            ShowGoal(map, manager);
            ShowSpawners(map, manager, viewContext);
            ShowCollisionBlock(map, manager);
        }

        private void ShowCollisionBlock(MapData map, GameManager manager)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/CollisionBlock");
            var offset = Vector2.one * 0.5f;
            foreach (var item in map.CollisionBlocks)
            {
                var obj = Instantiate(prefab);
                obj.transform.position = item + offset;
                obj.transform.SetParent(manager.transform);
            }
        }

        private void ShowSpawners(MapData map, GameManager manager, AdventureViewContext viewContext)
        {
            var prefab = Resources.Load<SpawnerController>("Prefabs/Character/Spawner");
            foreach (var spawner in map.Spawners)
            {
                var obj = Instantiate(prefab);
                obj.Initialize(spawner, viewContext);
                obj.transform.SetParent(manager.transform);
            }
        }

        private void ShowGoal(MapData map, GameManager manager)
        {
            var goalPrefab = Resources.Load<Goal>("Prefabs/Dungeon/Goal_Control");
            Goal = Instantiate(goalPrefab);
            Goal.transform.position = map.GoalLocation + new Vector2(0.5f, 0.5f);
            Goal.transform.SetParent(RootObjectRepository.I.ManagerDraw.transform);
            Goal.Initialize(manager.EventFacade);
        }

        private void OnDestroy()
		{
            if (Goal != null)
			{
				Destroy(Goal.gameObject);
            }
            Destroy(Maze.gameObject);
            Destroy(DebugRoot.gameObject);
        }

        private void ShowRooms(MapData map, GameObject maze)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Room_Control");
            DebugRoot = new GameObject();
            DebugRoot.name = "Debug_MazeViewer";

            foreach (var division in map.Divisions)
            {
                var obj = InstantiateRect(DebugRoot, prefab, division.Bound);
                obj.GetComponent<BoxCollider2D>().enabled = false;

                InstantiateRect(maze, prefab, division.Room);
                foreach (var connection in division.Connections)
                {
                    foreach (var segment in connection.Path.GetRooms())
                    {
                        InstantiateRect(maze, prefab, segment);
                    }
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