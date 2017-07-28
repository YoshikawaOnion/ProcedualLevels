using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public abstract class RoomGenStrategy
    {
        public abstract IEnumerable<MapDivision> GenerateRooms(MapRectangle root);
    }
}