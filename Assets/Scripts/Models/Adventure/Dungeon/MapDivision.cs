using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// マップ内の区画を表すクラス。
    /// </summary>
    public class MapDivision
    {
        /// <summary>
        /// 区画の範囲を取得または設定します。
        /// </summary>
        public MapRectangle Bound { get; set; }
        /// <summary>
        /// 区画内にある部屋の範囲を取得または設定します。
        /// </summary>
        public MapRectangle Room { get; set; }
        /// <summary>
        /// 区画のインデックスを取得または設定します。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// この部屋から伸びている通路を取得または設定します。
		/// </summary>
        [Obsolete]
        public List<MapConnection> Connections { get; private set; }
        public int ReducingMarker { get; set; }
    }
}