using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapData
{
    public List<MapDivision> Divisions { get; private set; }

    public MapData()
    {
        Divisions = new List<MapDivision>();
    }
}
