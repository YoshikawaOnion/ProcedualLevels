using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public abstract class EnemyGenStrategy
    {
        public abstract void PlaceEnemies(MapData map, EnemiesAbility ability, IAdventureView view);
    }
}