﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 部屋の底から水平に通路を伸びる通路を表すクラス。
    /// </summary>
    public class OnBottomHorizontalPath : IMapPath
    {
        /// <summary>
        /// 始点の部屋の底から水平に伸びている部分の矩形を取得します。
        /// </summary>
        public MapRectangle StartPath { get; private set; }
        /// <summary>
        /// 終点の部屋までの高さの差を補うために垂直に伸びている部分の矩形を取得します。
        /// 高さを補う必要がなかった場合は null です。
        /// </summary>
        public MapRectangle EndPath { get; private set; }

        public bool DebugMark { get; set; }

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

            if (EndPath != null)
            {
                var majorLeftBlockX = EndPath.Left;
                var majorRightBlockX = EndPath.Right - 1;
                if (EndPath.Top > StartPath.Top)  // 上に伸びる場合
                {
                    var majorBlockY = connection.TopDivision.Room.Bottom;
                    yield return new Vector2(majorLeftBlockX, majorBlockY);
                    yield return new Vector2(majorRightBlockX, majorBlockY);
                }
                else if(EndPath.Bottom < StartPath.Bottom)  // 下に伸びる場合
                {
                    var majorBlockY = connection.TopDivision.Room.Top - 1;
                    yield return new Vector2(majorLeftBlockX, majorBlockY);
                    yield return new Vector2(majorRightBlockX, majorBlockY);
                }

                yield return new Vector2(majorLeftBlockX, StartPath.Bottom);
                yield return new Vector2(majorLeftBlockX, StartPath.Top - 1);

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

        /// <summary>
        /// この通路に属する空間のコレクションを取得します。
        /// </summary>
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