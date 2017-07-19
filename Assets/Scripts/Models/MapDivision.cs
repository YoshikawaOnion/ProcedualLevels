using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class MapDivision
{
    public MapRectangle Bound { get; set; }
    public MapRectangle Room { get; set; }

    public List<Tuple<MapDivision, MapPath>> ConnectedDivisions { get; private set; }
    public int ReducingMarker { get; set; }

    public MapDivision()
    {
        ConnectedDivisions = new List<Tuple<MapDivision, MapPath>>();
    }
}
