using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// マップ内の足場を表すクラス。
    /// </summary>
    public class MapPlatform
    {
        /// <summary>
        /// 足場の存在する左端の位置を取得または設定します。
        /// </summary>
        public int Left { get; set; }
        /// <summary>
        /// 足場の存在する右端の位置を取得または設定します。
        /// </summary>
        public int Right { get; set; }
        /// <summary>
        /// 足場の存在する高さを取得または設定します。
        /// </summary>
        public int Bottom { get; set; }
    }
}