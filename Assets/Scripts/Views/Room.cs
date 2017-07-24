using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Room : MonoBehaviour
{
    public void PutWalls(int width, int height)
    {
        var prefab = Resources.Load<GameObject>("Prefabs/Wall");
        var wallLeft = Instantiate(prefab);
        var wallRight = Instantiate(prefab);
        var ceil = Instantiate(prefab);
        var floor = Instantiate(prefab);

        wallLeft.transform.parent = transform;
        wallLeft.transform.localPosition = new Vector3(-width / 2 - 0.5f, 0, 0);
        wallLeft.transform.localScale = new Vector3(1, height, 1);

        wallRight.transform.SetParent(transform);
        wallRight.transform.localPosition = new Vector3(width / 2 + 0.5f, 0, 0);
        wallRight.transform.localScale = new Vector3(1, height, 1);

        ceil.transform.SetParent(transform);
        ceil.transform.localPosition = new Vector3(0, height / 2 + 0.5f, 0);
        ceil.transform.localScale = new Vector3(width, 1, 1);

        floor.transform.SetParent(transform);
        floor.transform.localPosition = new Vector3(0, -height / 2 - 0.5f, 0);
        floor.transform.localScale = new Vector3(width, 1, 1);
    }
}
