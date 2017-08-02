using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class OnGoalEnemyGenStrategy : EnemyGenStrategy
    {
        public override void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index)
        {
            var bossRoomList = new List<int>();
            int indexCopy = index;
            Action<int, int, int> addEnemy = (left, bottom, roomIndex) =>
            {
                if (bossRoomList.Contains(roomIndex))
                {
                    return;
                }

                var pos = new Vector2(left + 3, bottom + 1);
                var enemy = new Enemy(indexCopy, pos, ability, view);
                map.Enemies.Add(enemy);
                bossRoomList.Add(roomIndex);
                ++indexCopy;
            };

            // 自分からゴール部屋へ通路が伸びていたらボスを配置
            foreach (var div in map.Divisions)
            {
                var connectionsForGoal = div.Connections
                                            .Where(x => x.TopDivision.Room
                                                   .IsInside(map.GoalLocation.x, map.GoalLocation.y));
                foreach (var connection in connectionsForGoal)
				{
                    var xPos = connection.Path.GetRooms().Min(x => x.Left);
                    int yPos = 0;
                    if (connection.TopDivision.Room.Bottom > div.Room.Bottom)
					{
						yPos = connection.Path.GetRooms().Min(x => x.Bottom);
                    }
                    else
                    {
                        yPos = connection.Path.GetRooms().Max(x => x.Bottom);
                    }
                    addEnemy(xPos, yPos, div.Index);
                }
            }

            // ゴール部屋へ通路が伸びている部屋から、その上下の部屋にも通路が伸びていたら上下の部屋にも配置
            //*/
            foreach (var div in map.Divisions)
            {
                var adjacentConnections = div.Connections
                                             .Where(x => x.TopDivision.Bound.Bottom == div.Bound.Top
                                                   && x.TopDivision.Bound.Left == div.Bound.Left);
                foreach (var connection in adjacentConnections)
                {
                    if (bossRoomList.Contains(div.Index))
                    {
                        var xPos = connection.Path.GetRooms().Min(x => x.Left);
                        var yPos = connection.Path.GetRooms().Max(x => x.Bottom);
                        addEnemy(xPos, yPos, connection.TopDivision.Index);
                    }
                    else if(bossRoomList.Contains(connection.TopDivision.Index))
                    {
                        var xPos = connection.Path.GetRooms().Min(x => x.Left);
                        var yPos = connection.Path.GetRooms().Min(x => x.Bottom);
                        addEnemy(xPos, yPos, div.Index);
                    }
                }
            }
            //*/

            index = indexCopy;
        }
    }
}