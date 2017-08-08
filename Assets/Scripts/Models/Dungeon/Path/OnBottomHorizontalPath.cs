using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomHorizontalPath : IMapPath
    {
        public MapRectangle BottomPath { get; private set; }
        public MapRectangle TopPath { get; private set; }

        public OnBottomHorizontalPath(MapRectangle bottomPath, MapRectangle topPath)
        {
            BottomPath = bottomPath;
            TopPath = topPath;
        }

        public IEnumerable<Vector2> GetCollisionBlocks()
        {
            yield break;
        }

        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return BottomPath;
            yield return TopPath;
        }
    }
}