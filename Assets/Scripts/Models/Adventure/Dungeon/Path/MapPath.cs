using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ProcedualLevels.Common;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 3つの矩形パーツからなるマップ内の通路を表すクラス。
    /// </summary>
    public class MapPath : IMapPath
    {
        /// <summary>
        /// 座標の大きな方の部屋から伸びている通路のパーツを取得または設定します。
        /// </summary>
        public MapRectangle TopPath { get; set; }
        /// <summary>
        /// 座標の小さな方の部屋から伸びている通路のパーツを取得または設定します。
        /// </summary>
        public MapRectangle BottomPath { get; set; }
        /// <summary>
        /// 2つの通路パーツを繋ぐ通路パーツを取得または設定します。
        /// </summary>
        /// <value>The connection.</value>
        public MapRectangle Connection { get; set; }

        /// <summary>
        /// 全ての通路パーツを列挙します。
        /// </summary>
        /// <returns>全ての通路パーツを列挙するコレクション。</returns>
        public IEnumerable<MapRectangle> GetRooms()
        {
            yield return BottomPath;
            yield return Connection;
            yield return TopPath;
        }

        /// <summary>
        /// MapPath の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="bottomPath">座標の小さな方から伸びている通路パーツ。</param>
        /// <param name="connection">他の2つの通路パーツを繋ぐ通路パーツ。</param>
        /// <param name="topPath">座標の大きな方から伸びている通路パーツ。</param>
        public MapPath(MapRectangle bottomPath, MapRectangle connection, MapRectangle topPath)
        {
            BottomPath = bottomPath;
            Connection = connection;
            TopPath = topPath;
        }

        /// <summary>
        /// この通路を複製します。
        /// </summary>
        /// <returns>複製された通路。</returns>
        internal MapPath Clone()
        {
            return new MapPath(BottomPath.Clone(), Connection.Clone(), TopPath.Clone());
        }

        public IEnumerable<Vector2> GetCollisionBlocks(MapData map, MapConnection connection)
        {
            yield break;
        }
    }
}