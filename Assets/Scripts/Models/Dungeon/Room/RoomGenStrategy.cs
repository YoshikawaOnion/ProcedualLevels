using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public abstract class RoomGenStrategy
	{
		protected RandomRoomGenAsset Asset { get; set; }
		protected int ChildBoundMinSize { get { return Asset.ChildBoundMinSize; } }
		protected int ParentBoundMinSize { get { return Asset.ParentBoundMinSize; } }
		protected int MarginSize { get { return Asset.MarginSize; } }
		protected int RoomMinSize { get { return Asset.RoomMinSize; } }
		protected int RoomMaxSize { get { return Asset.RoomMaxSize; } }

        public RoomGenStrategy()
		{
			Asset = Resources.Load<RandomRoomGenAsset>("Assets/RandomRoomGenAsset");
        }

        public abstract IEnumerable<MapDivision> GenerateRooms(MapRectangle root);

		/// <summary>
		/// 区画の中に部屋を生成します。
		/// </summary>
		/// <returns>生成した部屋の範囲。</returns>
		/// <param name="bound">部屋を生成できる区画の範囲。</param>
		protected virtual MapRectangle CreateRoom(MapRectangle bound)
		{
			var room = bound.Clone();

			var widthMinReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMaxSize);
			var heightMinReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMaxSize);
			var widthMaxReduce = Mathf.Max(2 * MarginSize, bound.Width - RoomMinSize);
			var heightMaxReduce = Mathf.Max(2 * MarginSize, bound.Height - RoomMinSize);
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