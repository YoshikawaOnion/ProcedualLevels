using UnityEngine;
using System.Collections;

public class MapView : MonoBehaviour
{
    [SerializeField]
    private int roomMinSize = 6;
    [SerializeField]
    private int parentRoomMinSize = 12;
    [SerializeField]
    private int marginSize = 4;

    public void Initialize(Camera camera)
	{
		var generator = new MapGenerator()
        {
            RoomMinSize = roomMinSize,
            ParentRoomMinSize = parentRoomMinSize,
            MarginSize = marginSize,
        };
        var leftBottom = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var rightTop = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
		var map = generator.GenerateMap(leftBottom, rightTop);

        var mazePrefab = Resources.Load<GameObject>("Prefabs/Maze");
        var maze = Instantiate(mazePrefab);

        var prefab = Resources.Load<GameObject>("Prefabs/Room");
        foreach (var room in map.Divisions)
        {
			InstantiateRect(maze, prefab, room.Room);
			foreach (var path in room.ConnectedDivisions)
			{
				foreach (var segment in path.Item2.Rooms)
				{
					InstantiateRect(maze, prefab, segment);
				}
			}
        }
    }

    private static void InstantiateRect(GameObject maze, GameObject prefab, MapRectangle room)
    {
        var obj = Instantiate(prefab);
        obj.transform.position = room.Position + room.Size / 2;
        obj.transform.localScale = room.Size;
        obj.transform.parent = maze.transform;
    }
}
