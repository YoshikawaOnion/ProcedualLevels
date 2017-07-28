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
			Func<MapRectangle, MapRectangle, bool> isAdjacentOnTop = (b, t) => b.Top == t.Bottom;

			foreach (var bottomDiv in map.Divisions)
			{
				var horizontalAdjacents = map.Divisions
                                             .Where(x => isAdjacentOnRight(bottomDiv.Bound, x.Bound));
				foreach (var topDiv in horizontalAdjacents)
				{
					var path = CreatePath(bottomDiv, topDiv, list);
					var connection = new MapConnection(bottomDiv, topDiv, path, true);
					bottomDiv.ConnectedDivisions.Add(connection);
					list.Add(connection);
				}

                /*
				var verticalAdjacents = map.Divisions
                                           .Where(x => isAdjacentOnTop(bottomDiv.Bound, x.Bound));
                foreach (var topDiv in verticalAdjacents)
                {
					var path = CreateVerticalPath(bottomDiv, topDiv, list);
					var connection = new MapConnection(bottomDiv, topDiv, path, true);
					bottomDiv.ConnectedDivisions.Add(connection);
					list.Add(connection);
                }
                //*/
            }
        }

        private MapPath CreateVerticalPath(MapDivision bottomDiv,
                                           MapDivision topDiv,
                                           List<MapConnection> list)
        {
            throw new NotImplementedException();
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

            return new GenericMapPath(list);
        }
    }
}