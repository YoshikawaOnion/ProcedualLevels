﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    private void Start()
    {
        var mapPrefab = Resources.Load<MapView>("Prefabs/Map");
        var mapObj = Instantiate(mapPrefab);
        mapObj.Initialize(camera);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Ball");
            var instance = Instantiate(prefab);
            var pos = camera.ScreenToWorldPoint(Input.mousePosition);
            instance.transform.position = new Vector3(pos.x, pos.y, -2);
        }
    }
}
