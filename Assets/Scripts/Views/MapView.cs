using UnityEngine;
using System.Collections;

public class MapView : MonoBehaviour
{
    public void Initialize(Camera camera, MapData map)
	{
        var mazePrefab = Resources.Load<GameObject>("Prefabs/Maze");
        var maze = Instantiate(mazePrefab);

        var prefab = Resources.Load<GameObject>("Prefabs/Room");
        var wallPrefab = Resources.Load<GameObject>("Prefabs/Area");
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
