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

        /// <summary>
        /// 所属する空間を与えて、OnBottomHorizontalPathの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="bottomPath">通路の始点から水平に伸びる通路。</param>
        /// <param name="topPath"><paramref name="bottomPath"/>の先から垂直に伸びる空間。
        /// nullの場合、水平な空間のみでこの通路が構成されます。</param>
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
            if (TopPath != null)
            {
                yield return TopPath;
            }
        }

        /// <summary>
        /// 二つの部屋を繋ぐ OnBottomHorizontalPath を生成します。
        /// </summary>
        /// <returns>The connection.</returns>
        /// <param name="bottomDiv">Bottom div.</param>
        /// <param name="topDiv">Top div.</param>
        public static OnBottomHorizontalPath CreateConnection(MapDivision bottomDiv, MapDivision topDiv)
        {
            var asset = AssetRepository.I.DungeonGenAsset;
            var horizontalPathThickness = asset.HorizontalPathThickness + 2 * asset.ColliderMargin;
            var verticalPathThickness = asset.VerticalPathThickness + 2 * asset.ColliderMargin;

            var path1 = new MapRectangle();
            path1.Bottom = bottomDiv.Room.Bottom;
            path1.Top = path1.Bottom + horizontalPathThickness;
            path1.Left = bottomDiv.Room.Right - verticalPathThickness;
            path1.Right = topDiv.Room.Left + verticalPathThickness;

            if (path1.Top - asset.ColliderMargin <= topDiv.Room.Bottom)
            {
                var path2 = new MapRectangle();
                path2.Bottom = path1.Bottom;
                path2.Top = topDiv.Room.Bottom + horizontalPathThickness;
                path2.Left = path1.Right - verticalPathThickness;
                path2.Right = path1.Right;
                return new OnBottomHorizontalPath(path1, path2);
            }
            else if (path1.Bottom + asset.ColliderMargin >= topDiv.Room.Top)
            {
                var path2 = new MapRectangle();
                path2.Bottom = topDiv.Room.Top - horizontalPathThickness;
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