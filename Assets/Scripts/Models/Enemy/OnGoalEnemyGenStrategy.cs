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
            foreach (var div in map.Divisions)
            {
                foreach (var connection in div.Connections)
                {
                    if (connection.TopDivision.Room.IsInside(map.GoalLocation.x, map.GoalLocation.y))
                    {
                        var path = connection.Path.GetRooms()
                                             .MinItem(x => x.Bottom);
                        var pos = new Vector2(path.Left, path.Bottom + 1);
                        var enemy = new Enemy(index, pos, ability, view);
                        map.Enemies.Add(enemy);
                        ++index;
                    }
                }
            }
        }
    }
}