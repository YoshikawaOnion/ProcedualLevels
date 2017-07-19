using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPath
{
    public List<MapRectangle> Rooms { get; private set; }

    public MapPath(IEnumerable<MapRectangle> rooms)
    {
        Rooms = new List<MapRectangle>(rooms);
    }
}
