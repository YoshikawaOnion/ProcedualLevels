using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class RandomEnemyGenStrategy : EnemyGenStrategy
    {
        private float Ratio { get; set; }
        private GameParameterAsset BattlerAsset { get; set; }
        private DungeonGenAsset DungeonAsset { get; set; }

        public RandomEnemyGenStrategy(float ratio,
                                      GameParameterAsset battlerAsset,
                                      DungeonGenAsset dungeonAsset)
        {
            Ratio = ratio;
            BattlerAsset = battlerAsset;
            DungeonAsset = dungeonAsset;
        }

        public override void PlaceEnemies(MapData map,
                                          EnemiesAbility ability,
                                          IAdventureView view,
                                          ref int index)
		{
			foreach (var item in map.Divisions)
			{
				var count = (int)((item.Room.Width - 1)
								  * (item.Room.Height - 1)
								  * Ratio);
				for (int i = 0; i < count; i++)
				{
					var pos = Helper.GetRandomLocation(item.Room, DungeonAsset.ColliderMargin);
					if (pos != map.GoalLocation
					   && (int)pos.x != (int)map.StartLocation.x)
					{
						var enemy = new Enemy(index, pos, ability, view)
						{
							DropPowerUp = i == 0
						};
						map.Enemies.Add(enemy);
						++index;
					}
				}
			}
        }
    }
}