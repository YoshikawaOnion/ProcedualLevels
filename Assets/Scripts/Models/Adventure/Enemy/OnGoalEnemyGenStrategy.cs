using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ゴールを塞ぐように敵キャラクターを配置するアルゴリズム。
    /// </summary>
    public class OnGoalEnemyGenStrategy : EnemyGenStrategy
    {
        public override void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index)
        {
            var bossRoomList = new List<int>();
            int indexCopy = index;    // ラムダ式は参照引数をキャプチャできないのでコピー
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

            var goalDiv = map.Divisions.First(x => x.Room
                                               .IsInside(map.GoalLocation.x, map.GoalLocation.y));
            var intersectedCons = map.Connections
                                     .Where(x => x.Path.GetRooms()
                                            .Any(y => y.IsIntersect(goalDiv.Room)));
            foreach (var connection in intersectedCons)
            {
                var rooms = connection.Path.GetRooms();
                var enemyX = rooms.Min(x => x.Left);
                int enemyY;
                if (connection.TopDivision.Room.Bottom > connection.BottomDivision.Room.Bottom)
                {
                    enemyY = rooms.Min(x => x.Bottom);
                }
                else
                {
                    enemyY = rooms.Max(x => x.Bottom);
                }
                addEnemy(enemyX, enemyY, connection.BottomDivision.Index);
            }

            index = indexCopy;
        }
    }
}