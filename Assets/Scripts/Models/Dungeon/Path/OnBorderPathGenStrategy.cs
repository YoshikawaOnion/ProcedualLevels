using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBorderPathGenStrategy : PathGenStrategy
    {
        public override IEnumerable<MapConnection> ConnectRooms(MapData map)
        {
            var con1 = ConnectRooms(map, true, (me, other) => me.Right == other.Left);
            var con2 = ConnectRooms(map, false, (me, other) => me.Top == other.Bottom);
            return con1.Concat(con2);
        }

        /// <summary>
        /// 指定した MapData に、MapData 内の部屋同士を結ぶ通路を追加します。
        /// </summary>
        /// <param name="map">更新する MapData。</param>
        /// <param name="horizontal"><c>true</c> の時、水平な通路を生成できる時のみ生成します。
        /// <c>false</c> の時、鉛直な通路を生成できる時のみ生成します。</param>
        /// <param name="isAdjacent">第二引数の部屋が第一引数の部屋から見て右上に隣接しているかどうかを判定する述語。</param>
        private IEnumerable<MapConnection> ConnectRooms(MapData map,
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
                    list.Add(connection);
                    yield return connection;
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
            var path1 = CreatePathSegment(bottomDiv, horizontal, false);
            var path2 = CreatePathSegment(topDiv, horizontal, true);
            MergePath(connections, bottomDiv, path1, horizontal, false);
            MergePath(connections, topDiv, path2, horizontal, true);

            int primaryThickness, secondaryThickness;
            AxisHelper.GetAligned(horizontal,
                                  ActualHorizontalPathThickness,
                                  ActualVerticalPathThickness,
                                  out primaryThickness,
                                  out secondaryThickness);

            var connection = new MapRectangle();
            var conProxy = new RectangleProxy(connection, horizontal);
            conProxy.PrimalMinor = path1.PrimalMajor - secondaryThickness;
            conProxy.PrimalMajor = path2.PrimalMinor + secondaryThickness;
            conProxy.SecondMinor = Mathf.Min(path1.SecondMinor, path2.SecondMinor);
            conProxy.SecondMajor = Mathf.Max(path1.SecondMajor, path2.SecondMajor);

            return new MapPath(path1.Original, connection, path2.Original);
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
        private RectangleProxy CreatePathSegment(MapDivision div, bool horizontal, bool isTopDiv)
        {
            var rect = new MapRectangle();
            var rectProxy = new RectangleProxy(rect, horizontal);
            var roomProxy = new RectangleProxy(div.Room, horizontal);
            var boundProxy = new RectangleProxy(div.Bound, horizontal);

            int primaryThickness, secondaryThickness;
            AxisHelper.GetAligned(horizontal,
                                  ActualHorizontalPathThickness,
                                  ActualVerticalPathThickness,
                                  out primaryThickness,
                                  out secondaryThickness);

            if (isTopDiv)
            {
                // こちら側は、境界に沿う通路の幅を確保するために Major を少し伸ばす
                rectProxy.PrimalMinor = boundProxy.PrimalMinor - secondaryThickness;
                rectProxy.PrimalMajor = roomProxy.PrimalMinor + secondaryThickness;
            }
            else
            {
                rectProxy.PrimalMinor = roomProxy.PrimalMajor - secondaryThickness;
                rectProxy.PrimalMajor = boundProxy.PrimalMajor;
            }
            var pos = Helper.GetRandomInRange(roomProxy.SecondMinor,
                                              roomProxy.SecondMajor - primaryThickness);
            rectProxy.SecondMinor = pos;
            rectProxy.SecondMajor = pos + primaryThickness;

            return rectProxy;
        }

        /// <summary>
        /// 指定した通路が伸びている元の部屋と同じ部屋から伸びている通路があれば、通路の開始点を一つにまとめます。
        /// </summary>
        /// <param name="connections">すでに生成されている部屋の接続情報のコレクション。</param>
        /// <param name="division">判定対象の通路が伸びている元の部屋。</param>
        /// <param name="path">まとめる対象の通路。</param>
        /// <param name="horizontal"><c>true</c> なら水平方向に、<c>false</c> なら垂直方向の通路とみなしてまとめます。</param>
        private void MergePath(IEnumerable<MapConnection> connections,
                               MapDivision division,
                               RectangleProxy path,
                               bool horizontal,
                               bool isTop)
        {
            Func<MapConnection, MapDivision> selectDivision = c =>
                isTop ? c.TopDivision : c.BottomDivision;
            Func<MapPath, MapRectangle> selectRect = p =>
                isTop ? p.TopPath : p.BottomPath;

            var connection = connections.FirstOrDefault(x => selectDivision(x).Index == division.Index);
            if (connection != null && connection.Horizontal == horizontal)
            {
                var clone = new RectangleProxy(selectRect((MapPath)connection.Path).Clone(), horizontal);
                path.SecondMinor = clone.SecondMinor;
                path.SecondMajor = clone.SecondMajor;
            }
        }
    }
}