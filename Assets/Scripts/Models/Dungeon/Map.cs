using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// マップデータを表すクラス。
    /// </summary>
    public class MapData
    {
		/// <summary>
		/// このマップに所属する区画のリストを取得します。
		/// </summary>
		public List<MapDivision> Divisions { get; private set; }
        /// <summary>
        /// このマップに所属する通路のリストを取得します。
        /// </summary>
		public List<MapConnection> Connections { get; set; }
        /// <summary>
        /// スタート地点の位置を取得または設定します。
        /// </summary>
        public Vector2 StartLocation { get; set; }
        /// <summary>
        /// ゴール地点の位置を取得または設定します。
        /// </summary>
        public Vector2 GoalLocation { get; set; }
        /// <summary>
        /// このマップに所属する敵キャラクターのリストを取得します。
        /// </summary>
        public List<Enemy> Enemies { get; private set; }
        /// <summary>
        /// このマップに所属する足場のリストを取得します。
        /// </summary>
        public List<MapPlatform> Platforms { get; private set; }
        /// <summary>
        /// このマップに所属するスポナーのリストを取得します。
        /// </summary>
        public List<Spawner> Spawners { get; private set; }
        /// <summary>
        /// このマップに所属する衝突判定用のブロックのリストを取得します。
        /// </summary>
        public List<Vector2> CollisionBlocks { get; private set; }
        /// <summary>
        /// このマップに所属するトゲブロックのリストを取得します。
        /// </summary>
        public List<Spike> Spikes { get; private set; }

        public MapData()
        {
            Divisions = new List<MapDivision>();
            Connections = new List<MapConnection>();
            Enemies = new List<Enemy>();
            Platforms = new List<MapPlatform>();
            Spawners = new List<Spawner>();
            CollisionBlocks = new List<Vector2>();
            Spikes = new List<Spike>();
        }

        /// <summary>
        /// 指定した位置がこのマップの部屋の範囲内かどうかを判定します。
        /// </summary>
        /// <returns>指定した位置がこのマップに属する部屋の内部ならば<c>true</c>、外部ならば<c>false</c>。</returns>
        /// <param name="left">判定する位置のx座標。</param>
        /// <param name="bottom">判定する位置のy座標。</param>
        public bool IsRoom(int left, int bottom)
        {
            foreach (var item in Divisions)
            {
                if (IsInRectanble(left, bottom, item.Room.ReduceOnEdge(1)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定した位置がこのマップの通路の範囲内かどうかを判定します。
        /// </summary>
        /// <returns>指定した位置がこのマップに属する通路の内部ならば<c>true</c>、外部ならば<c>false</c>。</returns>
        /// <param name="left">Left.</param>
        /// <param name="bottom">Bottom.</param>
        public bool IsPath(int left, int bottom)
        {
            foreach (var connection in Connections)
            {
                foreach (var path in connection.Path.GetRooms())
                {
                    if (IsInRectanble(left, bottom, path.ReduceOnEdge(1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 指定した位置が矩形の範囲内かどうかを判定します。
        /// </summary>
        /// <returns>指定した位置が矩形の内部ならば<c>true</c>、外部ならば<c>false</c>。</returns>
        /// <param name="left">判定する位置のx座標。</param>
        /// <param name="bottom">判定する位置のy座標。</param>
        /// <param name="room"><paramref name="left"/>, <paramref name="bottom"/> で指定した位置を含むかどうかが判定される矩形。</param>
        private bool IsInRectanble(int left, int bottom, MapRectangle room)
        {
            if (left >= room.Left && bottom >= room.Bottom
               && left < room.Right && bottom < room.Top)
            {
                return true;
            }
            return false;
        }
    }
}