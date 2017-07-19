﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int worldWidth = 1920;
    [SerializeField]
    private int worldHeight = 1080;
    [SerializeField]
    private int roomMinSize = 24;

    [SerializeField]
    private Camera camera;

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
		};
		//var leftBottom = new Vector2(-WorldWidth / 2, -WorldHeight / 2);
		//var rightTop = new Vector2(WorldWidth / 2, WorldHeight / 2);
		var leftBottom = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
		var rightTop = camera.ViewportToWorldPoint(new Vector3(2, 2, 0));
		var map = generator.GenerateMap(leftBottom, rightTop);

        var mapPrefab = Resources.Load<MapView>("Prefabs/Map");
        var mapObj = Instantiate(mapPrefab);
        mapObj.Initialize(camera, map);

        var playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        player = Instantiate(playerPrefab);
        player.transform.position = map.Divisions[0].Room.Position
            + map.Divisions[0].Room.Size / 2;
        player.transform.SetPositionZ(-2);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Ball");
            var instance = Instantiate(prefab);
            var pos = camera.ScreenToWorldPoint(Input.mousePosition);
            instance.transform.position = new Vector3(pos.x, pos.y, -2);
        }

        camera.transform.position = player.transform.position.ZReplacedBy(-10);
    }
}
