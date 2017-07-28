using UnityEngine;
using System.Collections;
using System;
using ProcedualLevels.Common;
using ProcedualLevels.Models;

namespace ProcedualLevels.Views
{
    public class MapView : MonoBehaviour
    {
        public void Initialize(MapData map, GameManager manager)
        {
            var mazePrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Maze_Control");
            var maze = Instantiate(mazePrefab);

            ShowRooms(map, maze);
            ShowPlatforms(map);

            var goalPrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Goal_Control");
            var goal = Instantiate(goalPrefab);
            goal.transform.position = map.GoalLocation;
            goal.transform.SetParent(manager.managerDraw.transform);
        }

        private static void ShowRooms(MapData map, GameObject maze)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Room_Control");
            foreach (var division in map.Divisions)
            {
                var obj = InstantiateRect(null, prefab, division.Bound);
                obj.transform.SetParent(null);
                obj.name = "Division";
                obj.GetComponent<BoxCollider2D>().enabled = false;

                InstantiateRect(maze, prefab, division.Room);
                foreach (var connection in division.ConnectedDivisions)
                {
                    foreach (var segment in connection.Path.GetRooms())
                    {
                        InstantiateRect(maze, prefab, segment);
                    }
                }
            }
        }

        private static void ShowPlatforms(MapData map)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Platform_Control");
            foreach (var platform in map.Platforms)
            {
                for (int i = platform.Left; i < platform.Right; i++)
                {
                    var obj = Instantiate(prefab);
                    obj.transform.position = new Vector3(i, platform.Bottom, -1);
                }
            }
        }

        private static GameObject InstantiateRect(GameObject maze, GameObject prefab, MapRectangle room)
        {
            var obj = Instantiate(prefab);
            obj.transform.position = room.Position + room.Size / 2;
            obj.transform.localScale = room.Size;
            if (maze != null)
            {
                obj.transform.parent = maze.transform;
            }
            return obj;
        }
    }
}