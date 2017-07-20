using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Views
{
    public class MapView : MonoBehaviour
    {
        public void Initialize(MapData map, GameManager manager)
        {
            var mazePrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Maze_Root");
            var maze = Instantiate(mazePrefab);

            var prefab = Resources.Load<GameObject>("Prefabs/Dungeon/Room_Root");
            var wallPrefab = Resources.Load<GameObject>("Prefabs/Debug/Area");
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

                //var w = InstantiateRect(null, wallPrefab, division.Bound);
                //w.transform.AddPositionZ(2);
            }

            var goalPrefab = Resources.Load<GameObject>("Prefabs/Dungeon/Goal_Control");
            var goal = Instantiate(goalPrefab);
            goal.transform.position = map.GoalLocation;
            goal.transform.SetParent(manager.managerDraw.transform);
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