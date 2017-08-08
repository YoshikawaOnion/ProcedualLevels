﻿using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomVerticalPath : IMapPath
    {
        public MapRectangle StartPath { get; private set; }
        public MapRectangle MiddlePath { get; private set; }
        public MapRectangle EndPath { get; private set; }

        public OnBottomVerticalPath(MapRectangle startPath, MapRectangle middlePath, MapRectangle endPath)
        {
            StartPath = startPath;
            MiddlePath = middlePath;
            EndPath = endPath;
        }

        public IEnumerable<Vector2> GetCollisionBlocks(MapData map, MapConnection connection)
        {
            {
                var minorBlockX = connection.BottomDivision.Room.Right - 1;
                var minorTopBlockY = StartPath.Top - 1;
                var minorBottomBlockY = StartPath.Bottom;
                yield return new Vector2(minorBlockX, minorTopBlockY);
                yield return new Vector2(minorBlockX, minorBottomBlockY);
            }
            {
                var majorBlockX = connection.TopDivision.Room.Right - 1;
                var majorTopBlockY = EndPath.Top - 1;
                var majorBottomBlockY = EndPath.Bottom;
                yield return new Vector2(majorBlockX, majorTopBlockY);
                yield return new Vector2(majorBlockX, majorBottomBlockY);
            }
            {
                var cornerBlockX = MiddlePath.Left;
                var cornerBottomBlockY = StartPath.Top - 1;
                var cornerTopBlockY = EndPath.Bottom;
                var cornerTopBlock2Y = EndPath.Top;
                yield return new Vector2(cornerBlockX, cornerBottomBlockY);
                yield return new Vector2(cornerBlockX, cornerTopBlockY);
                yield return new Vector2(cornerBlockX, cornerTopBlock2Y);
            }

            foreach (var div in map.Divisions)
            {
                var room = div.Room;
                if (room.Left <= MiddlePath.Right && MiddlePath.Right <= room.Right)
                {
                    var x = MiddlePath.Right - 1;
                    if (MiddlePath.Bottom <= room.Bottom && room.Bottom <= MiddlePath.Top)
                    {
                        var y = room.Bottom;
                        yield return new Vector2(x, y);
                    }
                    if (MiddlePath.Bottom <= room.Top && room.Top <= MiddlePath.Top)
                    {
                        var y = room.Top - 1;
                        yield return new Vector2(x, y);
                    }
                }
            }
        }

        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return StartPath;
            yield return MiddlePath;
            yield return EndPath;
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