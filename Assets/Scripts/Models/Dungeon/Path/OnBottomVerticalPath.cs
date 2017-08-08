using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomVerticalPath : IMapPath
    {
        public MapRectangle BottomPath { get; private set; }
        public MapRectangle MiddlePath { get; private set; }
        public MapRectangle TopPath { get; private set; }

        public OnBottomVerticalPath(MapRectangle bottomPath, MapRectangle middlePath, MapRectangle topPath)
        {
            BottomPath = bottomPath;
            MiddlePath = middlePath;
            TopPath = topPath;
        }

        public IEnumerable<Vector2> GetCollisionBlocks()
        {
            yield break;
        }

        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return BottomPath;
            yield return MiddlePath;
            yield return TopPath;
        }
    }
}