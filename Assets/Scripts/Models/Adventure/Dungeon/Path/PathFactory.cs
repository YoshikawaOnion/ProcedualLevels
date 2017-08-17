using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 通路を生成するファクトリ クラス。
    /// </summary>
    public static class PathFactory
    {
        /// <summary>
        /// 部屋の底から水平に伸びる通路を生成します。
        /// </summary>
        /// <returns>生成した通路。</returns>
        /// <param name="startDiv">始点となる左側の区画。</param>
        /// <param name="endDiv">終点となる右側の区画。</param>
        public static IMapPath CreateBottomHorizontalPath(MapDivision startDiv, MapDivision endDiv)
        {
            return OnBottomHorizontalPath.CreateConnection(startDiv, endDiv);
        }

        /// <summary>
        /// 部屋の底から鉛直に伸びる通路を生成します。
        /// </summary>
        /// <returns>生成した通路。</returns>
        /// <param name="startDiv">始点となる下側の区画。</param>
        /// <param name="endDiv">終点となる上側の区画。</param>
        public static IMapPath CreateBottomVerticalPath(MapDivision startDiv, MapDivision endDiv)
        {
            return OnBottomVerticalPath.CreatePath(startDiv, endDiv);
        }
    }
}