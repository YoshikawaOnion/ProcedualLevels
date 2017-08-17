using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 左から右に向かって部屋を生成するアルゴリズムを提供するクラス。
    /// </summary>
    public class HorizontalRoomGenStrategy : RoomGenStrategy
	{
        private HorizontalRoomGenAsset RoomGenAsset { get; set; }

        public HorizontalRoomGenStrategy()
        {
            RoomGenAsset = Resources.Load<HorizontalRoomGenAsset>("Assets/HorizontalRoomGenAsset");
        }

        /// <summary>
        /// ダンジョンに部屋を生成します。
        /// </summary>
        /// <returns>生成した部屋のコレクション。</returns>
        /// <param name="root">ダンジョンの大きさを表す矩形範囲。</param>
        public override IEnumerable<MapDivision> GenerateRooms(MapRectangle root)
		{
			var divisions = GenerateDivisions(root, true)
				.ToArray();

            var list = new List<MapDivision>();
			foreach (var div in divisions)
			{
				var room = CreateRoom(div);
				var element = new MapDivision()
				{
					Bound = div,
					Room = room,
					Index = list.Count
				};
                element.Bound.Name = "Division";
                element.Room.Name = "Room";
                list.Add(element);
			}

            return list;
		}

        /// <summary>
        /// 部屋どうしの下端を揃えます。
        /// </summary>
        /// <param name="list">揃える部屋のリスト。</param>
        private void AlignBottom(List<MapDivision> list)
		{
            foreach (var item in list)
			{
				var adjacent = list.FirstOrDefault(x => x.Bound.Left == item.Bound.Right);
                if (adjacent != null
                    && UnityEngine.Random.value <= 1
                    && adjacent.Room.Bottom < item.Room.Top - RoomMinHeight
                    && adjacent.Room.Bottom >= item.Bound.Bottom)
                {
                    var distance = item.Room.Bottom - adjacent.Room.Bottom;
                    item.Room.Bottom -= distance;
                    item.Room.Top -= distance;
                }
            }
        }

        /// <summary>
        /// 指定した範囲を分ける区画を生成します。
        /// </summary>
        /// <returns>分割された区画のコレクション。</returns>
        /// <param name="root">分割する範囲を表す矩形範囲。</param>
        /// <param name="horizontal"><c>true</c> を指定すると、最初の分割を水平に分割します。</param>
        private IEnumerable<MapRectangle> GenerateDivisions(MapRectangle root, bool horizontal)
        {
            MapRectangle child = new MapRectangle();
			var sizeToBeDivided = ChildBoundMinSize + ParentBoundMinSize;

			var childProxy = new RectangleProxy(child, horizontal);
            var rootProxy = new RectangleProxy(root, horizontal);

            if (rootProxy.PrimalLength < sizeToBeDivided
                || (!horizontal && UnityEngine.Random.value <= 1 - RoomGenAsset.VerticalSplitProbability))
            {
                yield return root;
                yield break;
            }

            var boundMax = horizontal ? Asset.BoundMaxWidth : Asset.BoundMaxHeight;

            var max = Mathf.Min(boundMax, rootProxy.PrimalLength - ChildBoundMinSize);
            var rand = UnityEngine.Random.Range(ParentBoundMinSize, max);
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
        protected override MapRectangle CreateRoom(MapRectangle bound)
		{
			var room = bound.Clone();

            var widthReduce = MarginSize;
            var heightReduce = Mathf.Max(MarginSize, bound.Height - RoomMaxHeight);

			room.Left += widthReduce / 2;
			room.Right -= widthReduce / 2;
			room.Bottom += heightReduce / 2;
			room.Top -= heightReduce / 2;

			return room;
        }
    }
}