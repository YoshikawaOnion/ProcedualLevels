using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ProcedualLevels.Models
{
    public class MapPath
    {
        public MapRectangle TopPath { get; set; }
        public MapRectangle BottomPath { get; set; }
        public MapRectangle Connection { get; set; }

        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return BottomPath;
            yield return Connection;
            yield return TopPath;
        }

        public MapPath(MapRectangle bottomPath, MapRectangle connection, MapRectangle topPath)
        {
            BottomPath = bottomPath;
            Connection = connection;
            TopPath = topPath;
        }

        internal MapPath Clone()
        {
            return new MapPath(BottomPath.Clone(), Connection.Clone(), TopPath.Clone());
        }
    }
}