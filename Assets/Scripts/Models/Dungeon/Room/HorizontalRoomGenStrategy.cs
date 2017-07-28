using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class HorizontalRoomGenStrategy : RoomGenStrategy
	{
		private RandomRoomGenAsset Asset { get; set; }
		private int ChildBoundMinSize { get { return Asset.ChildBoundMinSize; } }
		private int ParentBoundMinSize { get { return Asset.ParentBoundMinSize; } }
		private int MarginSize { get { return Asset.MarginSize; } }
		private int RoomMinSize { get { return Asset.RoomMinSize; } }
		private int RoomMaxSize { get { return Asset.RoomMaxSize; } }

		public HorizontalRoomGenStrategy()
		{
			Asset = Resources.Load<RandomRoomGenAsset>("Assets/RandomRoomGenAsset");
		}

        public override IEnumerable<MapDivision> GenerateRooms(MapRectangle root)
		{
			var divisions = GenerateDivisions(root, true)
				.ToArray();

            Debug.Log(divisions.Length);

            var list = new List<MapDivision>();
			foreach (var div in divisions)
			{
				var room = CreateRoom(div, list);
				var element = new MapDivision()
				{
					Bound = div,
					Room = room,
					Index = list.Count
				};
                list.Add(element);
			}

            return list;
		}

        private IEnumerable<MapRectangle> GenerateDivisions(MapRectangle root, bool horizontal)
        {
            MapRectangle child = new MapRectangle();
			var sizeToBeDivided = ChildBoundMinSize + ParentBoundMinSize;

			var childProxy = new RectangleProxy(child, horizontal);
            var rootProxy = new RectangleProxy(root, horizontal);

            if (rootProxy.PrimalLength < sizeToBeDivided
               || (!horizontal && UnityEngine.Random.value <= 0.5f))
            {
                yield return root;
                yield break;
            }

            var max = Mathf.Min(Asset.BoundMaxSize, rootProxy.PrimalLength - ChildBoundMinSize);
            var rand = Helper.GetRandomInRange(ParentBoundMinSize, max);
			childProxy.PrimalMinor = rootProxy.PrimalMinor + rand;
			childProxy.PrimalMajor = rootProxy.PrimalMajor;
			childProxy.SecondMinor = rootProxy.SecondMinor;
			childProxy.SecondMajor = rootProxy.SecondMajor;
			rootProxy.PrimalMajor = childProxy.PrimalMinor;

			if (horizontal)
			{
				foreach (var item in GenerateDivisions(root, false))
				{
					yield return item;
				}
			}
            else
            {
                yield return root;
            }

            foreach (var item in GenerateDivisions(child, horizontal))
            {
                yield return item;
            }
        }

		/// <summary>
		/// 区画の中に部屋を生成します。
		/// </summary>
		/// <returns>生成した部屋の範囲。</returns>
		/// <param name="bound">部屋を生成できる区画の範囲。</param>
		private MapRectangle CreateRoom(MapRectangle bound, IEnumerable<MapDivision> divisions)
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

            var adjacent = divisions.FirstOrDefault(x => x.Bound.Left == bound.Right);
            if (adjacent != null
                && UnityEngine.Random.value <= 0.95f
                && adjacent.Room.Bottom < room.Top - RoomMinSize)
            {
                room.Bottom = adjacent.Room.Bottom;
            }

            return room;
		}
    }
}