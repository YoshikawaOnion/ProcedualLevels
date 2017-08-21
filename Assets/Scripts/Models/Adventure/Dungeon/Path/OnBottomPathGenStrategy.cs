using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 部屋の底から水平に通路を伸ばすアルゴリズムを提供するクラス。
    /// </summary>
    public class OnBottomPathGenStrategy : PathGenStrategy
    {
        /// <summary>
        /// 部屋を通路で接続します。
        /// </summary>
        /// <returns>生成した接続のコレクション。</returns>
        /// <param name="map">接続すべき部屋の情報を提供するマップデータ。</param>
        public override IEnumerable<MapConnection> ConnectRooms(MapData map)
        {
            Func<MapRectangle, MapRectangle, bool> isAdjacentOnRight = (b, t) => b.Right == t.Left;
            Func<MapRectangle, MapRectangle, bool> isAdjacentOnTop = (b, t) => b.Top == t.Bottom
                                                                                && b.Left == t.Left;
            foreach (var bottomDiv in map.Divisions)
            {
                var horizontalAdjacents = map.Divisions
                                             .Where(x => isAdjacentOnRight(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in horizontalAdjacents)
                {
                    var path = CreatePath(bottomDiv, topDiv);
                    if (GetDivisionHavingSlimWall(map, path).Any())
                    {
                    }
                    yield return new MapConnection(bottomDiv, topDiv, path, true);
                }

                var verticalAdjacents = map.Divisions
                                           .Where(x => isAdjacentOnTop(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in verticalAdjacents)
                {
                    if (topDiv.Room.Right <= bottomDiv.Room.Right)
                    {
                        var path = CreateVerticalPath(bottomDiv, topDiv);
                        if (IsThereSlimWall(map, path))
                        {
                            // 厚さ1マスの天井などができるような通路は生成しない
                            continue;
                        }
                        yield return new MapConnection(bottomDiv, topDiv, path, true);
                    }
                }
            }
            Debug.Log("Path Generated");
        }

        private bool IsThereSlimWall(MapData map, IMapPath path)
        {
            return map.Divisions.Any(x =>
            {
                return path.GetRooms().Any(y => x.Room.Bottom - y.Top < 0
                                          || x.Room.Top - y.Bottom > 0);
            });
        }

        private IEnumerable<MapDivision> GetDivisionHavingSlimWall(MapData map, OnBottomHorizontalPath path)
        {
            return map.Divisions.Where(x =>
            {
                return path.EndPath != null
                           && (x.Room.Top - path.EndPath.Bottom > 0
                               || x.Room.Bottom - path.EndPath.Top < 0)
                           && (x.Room.Bottom - path.StartPath.Top < 0
                               || x.Room.Top - path.StartPath.Bottom > 0);
            });
        }

        private IMapPath CreateVerticalPath(MapDivision bottomDiv, MapDivision topDiv)
        {
            return PathFactory.CreateBottomVerticalPath(bottomDiv, topDiv);
        }

        private OnBottomHorizontalPath CreatePath(MapDivision bottomDiv, MapDivision topDiv)
        {
            return PathFactory.CreateBottomHorizontalPath(bottomDiv, topDiv);
        }
    }
}