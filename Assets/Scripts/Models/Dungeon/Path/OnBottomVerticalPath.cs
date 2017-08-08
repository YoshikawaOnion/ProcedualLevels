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

        public static OnBottomVerticalPath CreatePath(MapDivision bottomDiv, MapDivision topDiv)
        {
            var asset = AssetRepository.I.DungeonGenAsset;
            var verticalPathThickness = asset.VerticalPathThickness + 2 * asset.ColliderMargin;
            var horizontalPathThickness = asset.HorizontalPathThickness + 2 * asset.ColliderMargin;
            var x = verticalPathThickness + bottomDiv.Room.Right;
            
            var path1 = new MapRectangle();
            path1.Bottom = bottomDiv.Room.Bottom;
            path1.Top = path1.Bottom + horizontalPathThickness;
            path1.Left = bottomDiv.Room.Right - verticalPathThickness;
            path1.Right = x;
            path1.Name = "Path1";

            var path2 = new MapRectangle();
            path2.Bottom = path1.Bottom;
            path2.Top = topDiv.Room.Bottom + horizontalPathThickness;
            path2.Left = path1.Right - verticalPathThickness;
            path2.Right = path1.Right;
            path2.Name = "Path2";

            var path3 = new MapRectangle();
            path3.Bottom = path2.Top - horizontalPathThickness;
            path3.Top = path2.Top;
            path3.Left = topDiv.Room.Right - verticalPathThickness;
            path3.Right = path2.Right;
            path3.Name = "Path3";

            return new OnBottomVerticalPath(path1, path2, path3);
        }
    }
}