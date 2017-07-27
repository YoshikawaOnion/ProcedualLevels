using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class RoomGenStrategy
    {
        private RandomRoomGenAsset Asset { get; set; }
        private int ChildBoundMinSize { get { return Asset.ChildBoundMinSize; } }
        private int ParentBoundMinSize { get { return Asset.ParentBoundMinSize; } }
        private int MarginSize { get { return Asset.MarginSize; } }
        private int RoomMinSize { get { return Asset.RoomMinSize; } }
        private int RoomMaxSize { get { return Asset.RoomMaxSize; } }

        public RoomGenStrategy()
        {
            Asset = Resources.Load<RandomRoomGenAsset>("Assets/RandomRoomGenAsset");
        }

        public IEnumerable<MapDivision> GenerateRooms(MapRectangle root)
		{
			var divisions = GenerateDivisions(root, true)
				.ToArray();

			int index = 0;
			foreach (var div in divisions)
			{
				var room = CreateRoom(div);
				yield return new MapDivision()
				{
					Bound = div,
					Room = room,
					Index = index
				};
				index++;
			}            
        }

		/// <summary>
		/// 指定した親区画を分割した区画のコレクションを、親から順に返します。
		/// </summary>
		/// <returns>分割済みの区画のコレクション。</returns>
		/// <param name="parent">ルートとなる親要素。</param>
		/// <param name="vertical"><c>true</c> を指定すると、垂直な線で分割します。</param>
		private IEnumerable<MapRectangle> GenerateDivisions(MapRectangle parent, bool vertical)
		{
			MapRectangle child = null;
			bool childIsToBeDivided = false;
			bool parentIsToBeDivided = false;
			var sizeToBeDivided = ChildBoundMinSize + ParentBoundMinSize;

            if (vertical)
			{
                if (parent.Width < sizeToBeDivided)
                {
                    yield return parent;
                    yield break;
                }
                var rand = Helper.GetRandomInRange(ParentBoundMinSize, parent.Width - ChildBoundMinSize);
				var left = parent.Left + rand;
				var right = parent.Right;
				child = new MapRectangle(left, right, parent.Bottom, parent.Top);
				parent.Right = left;

				childIsToBeDivided = child.Height > sizeToBeDivided;
				parentIsToBeDivided = parent.Height > sizeToBeDivided;
			}
			else
			{
				if (parent.Height < sizeToBeDivided)
				{
					yield return parent;
                    yield break;
				}
				var rand = Helper.GetRandomInRange(ParentBoundMinSize, parent.Height - ChildBoundMinSize);
				var bottom = parent.Bottom + rand;
				var top = parent.Top;
				child = new MapRectangle(parent.Left, parent.Right, bottom, top);
				parent.Top = bottom;

				childIsToBeDivided = child.Width > sizeToBeDivided;
				parentIsToBeDivided = parent.Width > sizeToBeDivided;
			}

			if (UnityEngine.Random.value < 0.2f)
			{
				parentIsToBeDivided = false;
			}
			if (UnityEngine.Random.value < 0.2f)
			{
				childIsToBeDivided = false;
			}

			if (parentIsToBeDivided)
			{
				foreach (var item in GenerateDivisions(parent, !vertical))
				{
					yield return item;
				}
			}
			else
			{
				yield return parent;
			}
			if (childIsToBeDivided)
			{
				foreach (var item in GenerateDivisions(child, !vertical))
				{
					yield return item;
				}
			}
			else
			{
				yield return child;
			}
		}

		/// <summary>
		/// 区画の中に部屋を生成します。
		/// </summary>
		/// <returns>生成した部屋の範囲。</returns>
		/// <param name="bound">部屋を生成できる区画の範囲。</param>
		private MapRectangle CreateRoom(MapRectangle bound)
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