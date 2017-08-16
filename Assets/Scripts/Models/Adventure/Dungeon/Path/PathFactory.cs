using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public static class PathFactory
    {
        public static IMapPath CreateBottomHorizontalPath(MapDivision startDiv, MapDivision endDiv)
        {
            return OnBottomHorizontalPath.CreateConnection(startDiv, endDiv);
        }

        public static IMapPath CreateBottomVerticalPath(MapDivision startDiv, MapDivision endDiv)
        {
            return OnBottomVerticalPath.CreatePath(startDiv, endDiv);
        }
    }
}