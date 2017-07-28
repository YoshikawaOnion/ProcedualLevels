using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public class HorizontalRoomGenStrategy : RoomGenStrategy
	{
        private HorizontalRoomGenAsset RoomGenAsset { get; set; }

        public HorizontalRoomGenStrategy()
        {
            RoomGenAsset = Resources.Load<HorizontalRoomGenAsset>("Assets/HorizontalRoomGenAsset");
        }

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
                list.Add(element);
			}

            AlignBottom(list);

            return list;
		}

        private void AlignBottom(List<MapDivision> list)
		{
            foreach (var item in list)
			{
				var adjacent = list.FirstOrDefault(x => x.Bound.Left == item.Bound.Right);
                if (adjacent != null
                    && UnityEngine.Random.value <= 1
                    && adjacent.Room.Bottom < item.Room.Top - RoomMinSize)
                {
                    var distance = item.Room.Bottom - adjacent.Room.Bottom;
                    item.Room.Bottom -= distance;
                    item.Room.Top -= distance;
                }
            }
        }

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
    }
}