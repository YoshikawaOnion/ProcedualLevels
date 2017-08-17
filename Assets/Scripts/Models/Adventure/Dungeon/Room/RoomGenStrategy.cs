using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 部屋を生成するアルゴリズムを提供するクラス。
    /// </summary>
    public abstract class RoomGenStrategy
	{
		protected RandomRoomGenAsset Asset { get; set; }
		protected int ChildBoundMinSize { get { return Asset.ChildBoundMinSize; } }
		protected int ParentBoundMinSize { get { return Asset.ParentBoundMinSize; } }
		protected int MarginSize { get { return Asset.MarginSize; } }
		protected int RoomMinWidth { get { return Asset.RoomMinWidth; } }
		protected int RoomMaxWidth { get { return Asset.RoomMaxWidth; } }
		protected int RoomMinHeight { get { return Asset.RoomMinHeight; } }
		protected int RoomMaxHeight { get { return Asset.RoomMaxHeight; } }

        public RoomGenStrategy()
		{
			Asset = Resources.Load<RandomRoomGenAsset>("Assets/RandomRoomGenAsset");
        }

        /// <summary>
        /// ダンジョンに部屋を生成します。
        /// </summary>
        /// <returns>生成した部屋のコレクション。</returns>
        /// <param name="root">ダンジョンの大きさを表す矩形範囲。</param>
        public abstract IEnumerable<MapDivision> GenerateRooms(MapRectangle root);

		/// <summary>
		/// 区画の中に部屋を生成します。
		/// </summary>
		/// <returns>生成した部屋の範囲。</returns>
		/// <param name="bound">部屋を生成できる区画の範囲。</param>
		protected virtual MapRectangle CreateRoom(MapRectangle bound)
		{
			var room = bound.Clone();

			var widthMinReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMaxWidth);
			var heightMinReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMaxHeight);
			var widthMaxReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMinWidth);
			var heightMaxReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMinHeight);
			var widthReduce = Helper.GetRandomInRange(widthMinReduce, widthMaxReduce);
			var heightReduce = Helper.GetRandomInRange(heightMinReduce, heightMaxReduce);
			room.Left += widthReduce / 2;
			room.Right -= widthReduce / 2;
			room.Bottom += heightReduce / 2;
			room.Top -= heightReduce / 2;

			return room;
		}
    }
}