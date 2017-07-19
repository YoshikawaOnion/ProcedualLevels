using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MapRect_
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Vector2 Position
    {
        get { return new Vector2(X, Y); }
    }

    public MapRect_()
        : this(0, 0, 0, 0)
    {
    }

    public MapRect_(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

public class MapDivision_
{
    public MapRect_ Outer { get; set; }
    public MapRect_ Room { get; set; }
    public int Index { get; set; }
}

public class Map_
{
    public List<MapDivision_> Divisions { get; set; }

    public Map_()
    {
        Divisions = new List<MapDivision_>();
    }
}

public class MapGenerator_ : MonoBehaviour
{
    public int minSize;

    [SerializeField]
    private Camera camera;

	void Start () {
        GenerateMap();
	}

    void GenerateMap()
    {
        var size = camera.ViewportToWorldPoint(new Vector3(1, 1, 1));
        var map = new Map_();

        var index = 0;
        bool vertical = true;

        var parent = CreateDivision(0, 0, (int)size.x * 2, (int)size.y * 2);
        parent.Index = index;
        map.Divisions.Add(parent);
        for (int i = 0; i < 3; i++)
        {
            ++index;
            MapDivision_ child = null;
            if (vertical)
            {
                var range = GetRange(parent.Outer.Width);
                var x = range.Item1 + parent.Outer.X;
                child = CreateDivision(x, parent.Outer.Y, range.Item2, parent.Outer.Height);
                if (range.Item1 < 0)
                {
                    parent.Outer.X += range.Item1 + parent.Outer.Width / 2;
                }
                else
				{
					parent.Outer.X += range.Item1 - parent.Outer.Width / 2;
                }
                parent.Outer.Width = Mathf.Abs(range.Item1) * 2;
            }
            else
			{
				var range = GetRange(parent.Outer.Height);
                var y = range.Item1 + parent.Outer.Y;
				child = CreateDivision(parent.Outer.X, y, parent.Outer.Width, range.Item2);
				if (range.Item1 < 0)
				{
					parent.Outer.Y += range.Item1 + parent.Outer.Height / 2;
				}
				else
				{
					parent.Outer.Y += range.Item1 - parent.Outer.Height / 2;
				}
                parent.Outer.Height = Mathf.Abs(range.Item1) * 2;
			}
			child.Index = index;
			parent = child;
            vertical = !vertical;
            map.Divisions.Add(parent);
        }

        foreach (var div in map.Divisions)
        {
            //ShowDivision(div);
        }
        foreach (var div in map.Divisions)
        {
            PutRoom(div);
        }
    }

    Tuple<int, int> GetRange(int parentSize)
    {
        var rand = (int)((Random.value * 2 - 1) * 0.2f * parentSize);
        if (rand < 0)
        {
            rand = Mathf.Clamp(rand, -parentSize / 2 + minSize, -minSize);
        }
        else
        {
            rand = Mathf.Clamp(rand, minSize, parentSize / 2 - minSize);
        }
        var offset = rand;
        var size = parentSize - Mathf.Abs(rand) * 2;
        return new Tuple<int, int>(offset, size);
    }

    MapDivision_ CreateDivision(int x, int y, int width, int height)
    {
        return new MapDivision_()
        {
            Outer = new MapRect_(x, y, width, height)
        };
    }

    void PutRoom(MapDivision_ division)
	{
		var prefab = Resources.Load<GameObject>("Prefabs/Room");
        var obj = Instantiate(prefab);
        var room = obj.GetComponent<Room>();

        room.transform.position = division.Outer.Position;
        Debug.Log(room.transform.position);
        room.PutWalls(division.Outer.Width - 6, division.Outer.Height - 6);
    }

    void ShowDivision(MapDivision_ division)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/Wall");
        var obj = Instantiate(prefab);
        obj.transform.localPosition = new Vector3(division.Outer.X, division.Outer.Y);
        obj.transform.localScale = new Vector3(division.Outer.Width, division.Outer.Height);

        var sprite = obj.GetComponent<SpriteRenderer>();
        var card = (float)division.Index / 5;
        sprite.color = new Color(card, card, card);
    }
}
