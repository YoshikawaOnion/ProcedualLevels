using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class MapConnection
    {
        public MapDivision TopDivision { get; set; }
        public MapDivision BottomDivision { get; set; }
        public MapPath Path { get; set; }
        public bool Horizontal { get; set; }

        public MapConnection(MapDivision bottomDivision, MapDivision topDivision, MapPath path, bool horizontal)
        {
            TopDivision = topDivision;
            BottomDivision = bottomDivision;
            Path = path;
            Horizontal = horizontal;
        }
    }
}