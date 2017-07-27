using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class PathGenStrategy
    {
        private DungeonGenAsset DungeonGenAsset { get; set; }
        private OnBorderPathGenAsset Asset { get; set; }
        private int VerticalPathThickness { get { return Asset.VerticalPathThickness; } }
        private int HorizontalPathThickness { get { return Asset.HorizontalPathThickness; } }
        private int ActualVerticalPathThickness
        {
            get
            {
                return VerticalPathThickness + DungeonGenAsset.ColliderMargin * 2;
            }
        }
        private int ActualHorizontalPathThickness
        {
            get
            {
                return HorizontalPathThickness + DungeonGenAsset.ColliderMargin * 2;
            }
        }

        public PathGenStrategy()
        {
            DungeonGenAsset = Resources.Load<DungeonGenAsset>("Assets/DungeonGenAsset");
            Asset = Resources.Load<OnBorderPathGenAsset>("Assets/OnBorderPathGenAsset");
        }

        public void ConnectRooms(MapData map)
        {
            ConnectRooms(map, true, (me, other) => me.Right == other.Left);
            ConnectRooms(map, false, (me, other) => me.Top == other.Bottom);
        }

        /// <summary>
        /// 指定した MapData に、MapData 内の部屋同士を結ぶ通路を追加します。
        /// </summary>
        /// <param name="map">更新する MapData。</param>
        /// <param name="horizontal"><c>true</c> の時、水平な通路を生成できる時のみ生成します。
        /// <c>false</c> の時、鉛直な通路を生成できる時のみ生成します。</param>
        /// <param name="isAdjacent">第二引数の部屋が第一引数の部屋から見て右上に隣接しているかどうかを判定する述語。</param>
        private void ConnectRooms(MapData map,
                                  bool horizontal,
                                  Func<MapRectangle, MapRectangle, bool> isAdjacent)
        {
            var list = new List<MapConnection>();

            foreach (var bottomDiv in map.Divisions)
            {
                var topDivs = map.Divisions
                                 .Where(x => isAdjacent(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in topDivs)
                {
                    var path = CreatePath(bottomDiv, topDiv, horizontal, list);
                    var connection = new MapConnection(bottomDiv, topDiv, path, horizontal);
                    bottomDiv.ConnectedDivisions.Add(connection);
                    list.Add(connection);
                }
            }
        }

        /// <summary>
        /// 指定した二つの区画を繋ぐ通路を生成します。
        /// </summary>
        /// <returns>生成された通路のサイズを表す矩形。</returns>
        /// <param name="bottomDiv">もう一方の区画と繋ぐ、座標の小さな方の区画。</param>
        /// <param name="topDiv">もう一方の区画と繋ぐ、座標の大きな方の区画。</param>
        /// <param name="horizontal"><c>true</c> を指定すると、水平な通路を生成します。
        /// <c>false</c>を指定すると、鉛直な通路を生成します。</param>
        private MapPath CreatePath(MapDivision bottomDiv,
                                   MapDivision topDiv,
                                   bool horizontal,
                                   IEnumerable<MapConnection> connections)
        {
            MapRectangle path1 = CreatePathSegment(bottomDiv, horizontal, false);
            MapRectangle path2 = CreatePathSegment(topDiv, horizontal, true);

            MergePath(connections,
                     c => c.BottomDivision,
                     bottomDiv,
                     p => p.BottomPath,
                     path1,
                     horizontal);
            MergePath(connections,
                     c => c.TopDivision,
                     topDiv,
                     p => p.TopPath,
                     path2,
                     horizontal);

            var connection = new MapRectangle();
            if (horizontal)
            {
                connection.Left = path1.Right - ActualVerticalPathThickness;
                connection.Right = path2.Left + ActualVerticalPathThickness;
                connection.Bottom = Mathf.Min(path1.Bottom, path2.Bottom);
                connection.Top = Mathf.Max(path1.Top, path2.Top);
            }
            else
            {
                connection.Bottom = path1.Top - ActualHorizontalPathThickness;
                connection.Top = path2.Bottom + ActualHorizontalPathThickness;
                connection.Left = Mathf.Min(path1.Left, path2.Left);
                connection.Right = Mathf.Max(path1.Right, path2.Right);
            }

            return new MapPath(path1, connection, path2);
        }

        /// <summary>
        /// 通路の一部となる、部屋から区画の境界線まで伸びる通路パーツを生成します。
        /// </summary>
        /// <returns>通路の一部となる通路パーツ。</returns>
        /// <param name="div">通路の始点となる部屋。</param>
        /// <param name="horizontal"><c>true</c> の時、水平に通路を伸ばします。
        /// <c>false</c>の時、鉛直に通路を伸ばします。</param>
        /// <param name="isTopDiv"><c>true</c> の時、繋ぎたい2つの部屋のうち座標の大きな方から伸ばしているとみなします。
        /// <c>false</c>の時、座標の小さな方から伸ばしているとみなします。</param>
        private MapRectangle CreatePathSegment(MapDivision div, bool horizontal, bool isTopDiv)
        {
            var rect = new MapRectangle();

            if (horizontal)
            {
                if (isTopDiv)
                {
                    rect.Left = div.Bound.Left - ActualVerticalPathThickness;
                    rect.Right = div.Room.Left + ActualVerticalPathThickness;
                }
                else
                {
                    // 2つの通路パーツが同じX座標まで伸びるようにするため、
                    // 伸ばすのは片方だけにする(こちらは伸ばさない)
                    rect.Left = div.Room.Right - ActualVerticalPathThickness;
                    rect.Right = div.Bound.Right;
                }
                var pos = Helper.GetRandomInRange(
                    div.Room.Bottom,
                    div.Room.Top - ActualHorizontalPathThickness);
                rect.Bottom = pos;
                rect.Top = pos + ActualHorizontalPathThickness;
            }
            else
            {
                if (isTopDiv)
                {
                    rect.Bottom = div.Bound.Bottom - ActualHorizontalPathThickness;
                    rect.Top = div.Room.Bottom + ActualHorizontalPathThickness;
                }
                else
                {
                    // 2つの通路パーツが同じY座標まで伸びるようにするため、
                    // 伸ばすのは片方だけにする(こちらは伸ばさない)
                    rect.Bottom = div.Room.Top - ActualHorizontalPathThickness;
                    rect.Top = div.Bound.Top;
                }
                var pos = Helper.GetRandomInRange(
                    div.Room.Left,
                    div.Room.Right - ActualVerticalPathThickness);
                rect.Left = pos;
                rect.Right = pos + ActualVerticalPathThickness;
            }

            return rect;
        }

        /// <summary>
        /// 指定した通路が伸びている元の部屋と同じ部屋から伸びている通路があれば、通路の開始点を一つにまとめます。
        /// </summary>
        /// <param name="connections">すでに生成されている部屋の接続情報のコレクション。</param>
        /// <param name="selectDivision">部屋の接続情報から判定対象の部屋を選ぶデリゲート。</param>
        /// <param name="division">判定対象の通路が伸びている元の部屋。</param>
        /// <param name="selectRect">同じ部屋から伸びていた通路から、まとめる先の矩形範囲を選ぶデリゲート。</param>
        /// <param name="path">まとめる対象の通路。</param>
        /// <param name="horizontal"><c>true</c> なら水平方向に、<c>false</c> なら垂直方向の通路とみなしてまとめます。</param>
        private void MergePath(IEnumerable<MapConnection> connections,
                               Func<MapConnection, MapDivision> selectDivision,
                               MapDivision division,
                               Func<MapPath, MapRectangle> selectRect,
                               MapRectangle path,
                              bool horizontal)
        {
            var connection = connections.FirstOrDefault(x => selectDivision(x).Index == division.Index);
            if (connection != null && connection.Horizontal == horizontal)
            {
                var clone = selectRect(connection.Path).Clone();
                if (horizontal)
                {
                    path.Bottom = clone.Bottom;
                    path.Top = clone.Top;
                }
                else
                {
                    path.Left = clone.Left;
                    path.Right = clone.Right;
                }
            }
        }
    }
}