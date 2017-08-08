using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomPathGenStrategy : PathGenStrategy
    {
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
                            continue;
                        }
                        yield return new MapConnection(bottomDiv, topDiv, path, true);
                    }
                }
            }
        }

        private bool IsThereSlimWall(MapData map, IMapPath path)
        {
            return map.Divisions.Any(x =>
            {
                return path.GetRooms().Any(y => x.Room.Bottom - y.Top < 0);
            });
        }

        private IMapPath CreateVerticalPath(MapDivision bottomDiv, MapDivision topDiv)
        {
            return PathFactory.CreateBottomVerticalPath(bottomDiv, topDiv);
        }

        private IMapPath CreatePath(MapDivision bottomDiv, MapDivision topDiv)
        {
            return PathFactory.CreateBottomHorizontalPath(bottomDiv, topDiv);
        }
    }
}