using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class RandomRoomGenStrategy : RoomGenStrategy
    {
        private RandomRoomGenAsset Asset { get; set; }
        private int ChildBoundMinSize { get { return Asset.ChildBoundMinSize; } }
        private int ParentBoundMinSize { get { return Asset.ParentBoundMinSize; } }
        private int MarginSize { get { return Asset.MarginSize; } }
        private int RoomMinSize { get { return Asset.RoomMinSize; } }
        private int RoomMaxSize { get { return Asset.RoomMaxSize; } }

        public RandomRoomGenStrategy()
        {
            Asset = Resources.Load<RandomRoomGenAsset>("Assets/RandomRoomGenAsset");
        }

        public override IEnumerable<MapDivision> GenerateRooms(MapRectangle root)
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
		/// <param name="horizontal"><c>true</c> を指定すると、水平方向に分割します。</param>
        private IEnumerable<MapRectangle> GenerateDivisions(MapRectangle parent, bool horizontal)
		{
			MapRectangle child = new MapRectangle();
			var sizeToBeDivided = ChildBoundMinSize + ParentBoundMinSize;

            var childProxy = new RectangleProxy(child, horizontal);
            var parentProxy = new RectangleProxy(parent, horizontal);

            if (parentProxy.PrimalLength < sizeToBeDivided || Random.value < 0.2f)
            {
                yield return parent;
                yield break;
            }
            else
            {
                var rand = Helper.GetRandomInRange(ParentBoundMinSize,
                                                   parentProxy.PrimalLength - ChildBoundMinSize);
                childProxy.PrimalMinor = parentProxy.PrimalMinor + rand;
                childProxy.PrimalMajor = parentProxy.PrimalMajor;
                childProxy.SecondMinor = parentProxy.SecondMinor;
                childProxy.SecondMajor = parentProxy.SecondMajor;
                parentProxy.PrimalMajor = childProxy.PrimalMinor;
            }

			foreach (var item in GenerateDivisions(parent, !horizontal))
			{
				yield return item;
			}
			foreach (var item in GenerateDivisions(child, !horizontal))
			{
				yield return item;
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