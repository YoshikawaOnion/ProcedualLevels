using System;
using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 矩形範囲の追加情報のないコレクションで通路を表現するクラス。
    /// </summary>
    public class GenericMapPath : IMapPath
    {
        private IEnumerable<MapRectangle> Rectangles { get; set; }

        public bool DebugMark { get; set; }

        public GenericMapPath(IEnumerable<MapRectangle> rectangles)
        {
            Rectangles = rectangles;
        }

        /// <summary>
        /// この通路に属する空間のコレクションを取得します。
        /// </summary>
        public IEnumerable<MapRectangle> GetRooms()
        {
            return Rectangles;
        }

        /// <summary>
        /// この通路に必要な、ダンジョンの角の衝突判定用ブロックの位置を決定します。
        /// </summary>
        /// <returns>衝突判定用ブロックの位置のコレクション。</returns>
        /// <param name="map">マップデータ。</param>
        /// <param name="connection">この通路が属する接続データ。</param>
        public IEnumerable<Vector2> GetCollisionBlocks(MapData map, MapConnection connection)
        {
            yield break;
        }
    }
}