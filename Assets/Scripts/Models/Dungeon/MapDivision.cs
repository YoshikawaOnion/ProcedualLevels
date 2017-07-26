using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace ProcedualLevels.Models
{
    public class MapDivision
    {
        public MapRectangle Bound { get; set; }
        public MapRectangle Room { get; set; }
        public int Index { get; set; }

        public List<MapConnection> ConnectedDivisions { get; private set; }
        public int ReducingMarker { get; set; }

        public MapDivision()
        {
            ConnectedDivisions = new List<MapConnection>();
        }
    }
}