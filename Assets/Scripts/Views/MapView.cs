using UnityEngine;
using System.Collections;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Views
{
    public class MapView : MonoBehaviour
    {
        public void Initialize(MapData map, GameManager manager)
        {
            var mazePrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Maze_Root");
            var maze = Instantiate(mazePrefab);

            ShowRooms(map, maze);

            var goalPrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Goal_Control");
            var goal = Instantiate(goalPrefab);
            goal.transform.position = map.GoalLocation;
            goal.transform.SetParent(manager.managerDraw.transform);
        }

        private void ShowEnemies(MapData map, GameManager manager)
        {
            var enemyPrefab = Resources.Load<GameObject>("Prefabs/Character/Enemy_Control");
            foreach (var p in map.EnemyLocations)
            {
                var obj = Instantiate(enemyPrefab);
                obj.transform.position = p.ToVector3().MergeZ(enemyPrefab.transform.position.z);
                obj.transform.SetParent(manager.managerDraw.transform);
            }
        }

        private static void ShowRooms(MapData map, GameObject maze)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Room_Root");
            foreach (var division in map.Divisions)
            {
                InstantiateRect(maze, prefab, division.Room);
                foreach (var connection in division.ConnectedDivisions)
                {
                    foreach (var segment in connection.Item2.Rooms)
                    {
                        InstantiateRect(maze, prefab, segment);
                    }
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