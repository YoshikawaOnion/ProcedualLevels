using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// ダンジョンの通路データの生成と管理に関する機能を提供するインターフェース。
    /// </summary>
    public interface IMapPath
    {
        /// <summary>
        /// この通路に属する空間のコレクションを取得します。
        /// </summary>
        IEnumerable<MapRectangle> GetRooms();
        /// <summary>
        /// この通路に必要な、ダンジョンの角の衝突判定用ブロックの位置を決定します。
        /// </summary>
        /// <returns>衝突判定用ブロックの位置のコレクション。</returns>
        /// <param name="map">マップデータ。</param>
        /// <param name="connection">この通路が属する接続データ。</param>
        IEnumerable<Vector2> GetCollisionBlocks(MapData map, MapConnection connection);
    }
}