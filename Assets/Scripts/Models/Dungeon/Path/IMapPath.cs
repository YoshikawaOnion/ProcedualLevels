using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public interface IMapPath
    {
        IEnumerable<MapRectangle> GetRooms();
        IEnumerable<Vector2> GetCollisionBlocks();
    }
}