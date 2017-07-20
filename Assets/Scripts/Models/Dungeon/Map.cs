using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapData
{
    public List<MapDivision> Divisions { get; private set; }
    public Vector2 StartLocation { get; set; }
    public Vector2 GoalLocation { get; set; }
    public List<Vector2> EnemyLocations { get; private set; }

    public MapData()
    {
        Divisions = new List<MapDivision>();
    }
}
