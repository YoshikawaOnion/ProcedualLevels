using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnBottomPathGenStrategy : PathGenStrategy
    {
        public override void ConnectRooms(MapData map)
        {
			var list = new List<MapConnection>();
			Func<MapRectangle, MapRectangle, bool> isAdjacentOnRight = (b, t) => b.Right == t.Left;
            Func<MapRectangle, MapRectangle, bool> isAdjacentOnTop = (b, t) => b.Top == t.Bottom
                                                                                && b.Left == t.Left;

			foreach (var bottomDiv in map.Divisions)
			{
                //*
				var horizontalAdjacents = map.Divisions
                                             .Where(x => isAdjacentOnRight(bottomDiv.Bound, x.Bound));
				foreach (var topDiv in horizontalAdjacents)
				{
					var path = CreatePath(bottomDiv, topDiv, list);
					var connection = new MapConnection(bottomDiv, topDiv, path, true);
					bottomDiv.Connections.Add(connection);
					list.Add(connection);
				}
                //*/

                //*
				var verticalAdjacents = map.Divisions
                                           .Where(x => isAdjacentOnTop(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in verticalAdjacents)
                {
                    if (topDiv.Room.Right <= bottomDiv.Room.Right)
					{
						var path = CreateVerticalPath(bottomDiv, topDiv, list);
                        if (IsThereSlimWall(map, path))
                        {
                            continue;
                        }
                        var connection = new MapConnection(bottomDiv, topDiv, path, true);
						bottomDiv.Connections.Add(connection);
						list.Add(connection);
                    }
                }
                //*/
            }
        }

        private bool IsThereSlimWall(MapData map, IMapPath path)
        {
            return map.Divisions.Any(x =>
            {
                return path.GetRooms().Any(y => x.Room.Bottom - y.Top < 0);
            });
        }

        private IMapPath CreateVerticalPath(MapDivision bottomDiv,
                                           MapDivision topDiv,
                                           List<MapConnection> connections)
		{
            var x = ActualVerticalPathThickness + bottomDiv.Room.Right;

            var path1 = new MapRectangle();
            path1.Bottom = bottomDiv.Room.Bottom;
            path1.Top = path1.Bottom + ActualHorizontalPathThickness;
            path1.Left = bottomDiv.Room.Right - ActualVerticalPathThickness;
            path1.Right = x;
            path1.Name = "Path1";

            var path2 = new MapRectangle();
            path2.Bottom = path1.Bottom;
            path2.Top = topDiv.Room.Bottom + ActualHorizontalPathThickness;
            path2.Left = path1.Right - ActualVerticalPathThickness;
            path2.Right = path1.Right;
            path2.Name = "Path2";

            var path3 = new MapRectangle();
            path3.Bottom = path2.Top - ActualHorizontalPathThickness;
            path3.Top = path2.Top;
            path3.Left = topDiv.Room.Right - ActualVerticalPathThickness;
            path3.Right = path2.Right;
            path3.Name = "Path3";

            return new GenericMapPath(new[] { path1, path2, path3 });
        }

        private IMapPath CreatePath(MapDivision bottomDiv,
                                   MapDivision topDiv,
                                   List<MapConnection> connections)
        {
            var list = new List<MapRectangle>();
            var path1 = new MapRectangle();

            path1.Bottom = bottomDiv.Room.Bottom;
            path1.Top = path1.Bottom + ActualHorizontalPathThickness;
            path1.Left = bottomDiv.Room.Right - ActualVerticalPathThickness;
            path1.Right = topDiv.Room.Left + ActualVerticalPathThickness;
            list.Add(path1);

            if (path1.Top - DungeonGenAsset.ColliderMargin <= topDiv.Room.Bottom)
            {
                var path2 = new MapRectangle();
                path2.Bottom = path1.Bottom;
                path2.Top = topDiv.Room.Bottom + ActualHorizontalPathThickness;
                path2.Left = path1.Right - ActualVerticalPathThickness;
                path2.Right = path1.Right;
                list.Add(path2);
            }
            else if(path1.Bottom >= topDiv.Room.Top - DungeonGenAsset.ColliderMargin)
			{
				var path2 = new MapRectangle();
                path2.Bottom = topDiv.Room.Top - ActualHorizontalPathThickness;
				path2.Top = path1.Top;
				path2.Left = path1.Right - ActualVerticalPathThickness;
				path2.Right = path1.Right;
				list.Add(path2);
            }

            return new GenericMapPath(list);
        }
    }
}