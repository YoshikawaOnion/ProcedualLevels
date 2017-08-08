﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomHorizontalPath : IMapPath
    {
        public MapRectangle StartPath { get; private set; }
        public MapRectangle EndPath { get; private set; }

        /// <summary>
        /// 所属する空間を与えて、OnBottomHorizontalPathの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="startPath">通路の始点から水平に伸びる通路。</param>
        /// <param name="endPath"><paramref name="startPath"/>の先から垂直に伸びる空間。
        /// nullの場合、水平な空間のみでこの通路が構成されます。</param>
        public OnBottomHorizontalPath(MapRectangle startPath, MapRectangle endPath)
        {
            StartPath = startPath;
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

            if (EndPath != null)
            {
                var majorLeftBlockX = EndPath.Left;
                var majorRightBlockX = EndPath.Right - 1;
                if (EndPath.Top > StartPath.Top)
                {
                    var majorBlockY = connection.TopDivision.Room.Bottom;
                    var cornerY = StartPath.Top - 1;
                    yield return new Vector2(majorLeftBlockX, majorBlockY);
                    yield return new Vector2(majorRightBlockX, majorBlockY);
                    yield return new Vector2(majorLeftBlockX, cornerY);
                }
                else if(EndPath.Bottom < StartPath.Bottom)
                {
                    var majorBlockY = connection.TopDivision.Room.Top - 1;
                    var cornerY = StartPath.Bottom;
                    yield return new Vector2(majorLeftBlockX, majorBlockY);
                    yield return new Vector2(majorRightBlockX, majorBlockY);
                    yield return new Vector2(majorLeftBlockX, cornerY);
                }

                foreach (var div in map.Divisions)
                {
                    var room = div.Room;
                    if (room.Left <= EndPath.Right && EndPath.Right <= room.Right)
                    {
                        var x = EndPath.Right - 1;
                        if (EndPath.Bottom <= room.Bottom && room.Bottom <= EndPath.Top)
                        {
                            var y = room.Bottom;
                            yield return new Vector2(x, y);
                        }
                        if (EndPath.Bottom <= room.Top && room.Top <= EndPath.Top)
                        {
                            var y = room.Top - 1;
                            yield return new Vector2(x, y);
                        }
                    }
                }
            }
            else
            {
                var majorBlockX = connection.TopDivision.Room.Left;
                var majorTopBlockY = StartPath.Top - 1;
                var majorBottomBlockY = StartPath.Bottom;
                yield return new Vector2(majorBlockX, majorTopBlockY);
                yield return new Vector2(majorBlockX, majorBottomBlockY);
            }
        }

        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return StartPath;
            if (EndPath != null)
            {
                yield return EndPath;
            }
        }

        /// <summary>
        /// 二つの部屋を繋ぐ OnBottomHorizontalPath を生成します。
        /// </summary>
        /// <returns>二つの部屋を繋ぐ OnBottomHorizontalPath。</returns>
        /// <param name="startDiv">通路の始点となる区画。</param>
        /// <param name="endDiv">通路の終点となる区画。</param>
        public static OnBottomHorizontalPath CreateConnection(MapDivision startDiv, MapDivision endDiv)
        {
            var asset = AssetRepository.I.DungeonGenAsset;
            var horizontalPathThickness = asset.HorizontalPathThickness + 2 * asset.ColliderMargin;
            var verticalPathThickness = asset.VerticalPathThickness + 2 * asset.ColliderMargin;

            var path1 = new MapRectangle();
            path1.Bottom = startDiv.Room.Bottom;
            path1.Top = path1.Bottom + horizontalPathThickness;
            path1.Left = startDiv.Room.Right - verticalPathThickness;
            path1.Right = endDiv.Room.Left + verticalPathThickness;

            if (path1.Top - asset.ColliderMargin <= endDiv.Room.Bottom + asset.ColliderMargin)
            {
                var path2 = new MapRectangle();
                path2.Bottom = path1.Bottom;
                path2.Top = endDiv.Room.Bottom + horizontalPathThickness;
                path2.Left = path1.Right - verticalPathThickness;
                path2.Right = path1.Right;
                return new OnBottomHorizontalPath(path1, path2);
            }
            else if (path1.Bottom + asset.ColliderMargin >= endDiv.Room.Top - asset.ColliderMargin)
            {
                var path2 = new MapRectangle();
                path2.Bottom = endDiv.Room.Top - horizontalPathThickness;
                path2.Top = path1.Top;
                path2.Left = path1.Right - verticalPathThickness;
                path2.Right = path1.Right;
                return new OnBottomHorizontalPath(path1, path2);
            }
            else
            {
                return new OnBottomHorizontalPath(path1, null);
            }
        }
    }
}