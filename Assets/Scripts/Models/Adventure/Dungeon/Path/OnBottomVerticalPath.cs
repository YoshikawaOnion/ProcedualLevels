﻿using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 部屋の底から、その上にある部屋の底へ接続する通路を表すクラス。
    /// </summary>
    public class OnBottomVerticalPath : IMapPath
    {
        /// <summary>
        /// 始点の部屋の底から水平に伸びる部分の矩形を取得します。
        /// </summary>
        public MapRectangle StartPath { get; private set; }
        /// <summary>
        /// <see cref="StartPath"/> の終点から鉛直に伸びる部分の矩形を取得します。
        /// </summary>
        public MapRectangle MiddlePath { get; private set; }
        /// <summary>
        /// <see cref="MiddlePath"/> の終点から終点の部屋の底へ水平に伸びる部分の矩形を取得します。
        /// </summary>
        /// <value>The end path.</value>
        public MapRectangle EndPath { get; private set; }

        public OnBottomVerticalPath(MapRectangle startPath, MapRectangle middlePath, MapRectangle endPath)
        {
            StartPath = startPath;
            MiddlePath = middlePath;
            EndPath = endPath;
        }

        /// <summary>
        /// この通路に必要な、ダンジョンの角の衝突判定用ブロックの位置を決定します。
        /// </summary>
        /// <returns>衝突判定用ブロックの位置のコレクション。</returns>
        /// <param name="map">マップデータ。</param>
        /// <param name="connection">この通路が属する接続データ。</param>
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

        /// <summary>
        /// この通路に属する空間のコレクションを取得します。
        /// </summary>
        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return StartPath;
            yield return MiddlePath;
            yield return EndPath;
        }

        /// <summary>
        /// 指定した2つの区画に属するを繋ぐ <see cref="OnBottomVerticalPath"/> を生成します。
        /// </summary>
        /// <returns>生成された通路。</returns>
        /// <param name="bottomDiv">下側の区画。</param>
        /// <param name="topDiv">上側の区画。</param>
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