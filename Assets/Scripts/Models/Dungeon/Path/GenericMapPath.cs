using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class GenericMapPath : IMapPath
    {
        public IEnumerable<MapRectangle> Rectangles { get; private set; }

        public GenericMapPath(IEnumerable<MapRectangle> rectangles)
        {
            Rectangles = rectangles;
        }

        public IEnumerable<MapRectangle> GetRooms()
        {
            return Rectangles;
        }
    }
}