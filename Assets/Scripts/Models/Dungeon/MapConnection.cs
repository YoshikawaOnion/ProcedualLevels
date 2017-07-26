using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 部屋同士の接続を表すクラス。
    /// </summary>
    public class MapConnection
    {
        /// <summary>
        /// 接続している、座標の大きな方の区画の範囲を取得または設定します。
        /// </summary>
        public MapDivision TopDivision { get; set; }
        /// <summary>
        /// 接続している、座標の小さな方の区画の範囲を取得または設定します。
        /// </summary>
        public MapDivision BottomDivision { get; set; }
        /// <summary>
        /// 2つの部屋を接続している通路を取得または設定します。
        /// </summary>
        public MapPath Path { get; set; }
        /// <summary>
        /// 通路が水平方向に伸びているかどうかを表す真偽値を取得または設定します。
        /// </summary>
        /// <value><c>true</c> の時は水平、<c>false</c> の時は鉛直。</value>
        public bool Horizontal { get; set; }

        public MapConnection(MapDivision bottomDivision, MapDivision topDivision, MapPath path, bool horizontal)
        {
            TopDivision = topDivision;
            BottomDivision = bottomDivision;
            Path = path;
            Horizontal = horizontal;
        }
    }
}