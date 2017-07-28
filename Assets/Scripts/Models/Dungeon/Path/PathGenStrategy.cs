using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public abstract class PathGenStrategy
    {
		protected DungeonGenAsset DungeonGenAsset { get; set; }
		protected OnBorderPathGenAsset Asset { get; set; }
		protected int VerticalPathThickness { get { return Asset.VerticalPathThickness; } }
		protected int HorizontalPathThickness { get { return Asset.HorizontalPathThickness; } }
		protected int ActualVerticalPathThickness
		{
			get
			{
				return VerticalPathThickness + DungeonGenAsset.ColliderMargin * 2;
			}
		}
		protected int ActualHorizontalPathThickness
		{
			get
			{
				return HorizontalPathThickness + DungeonGenAsset.ColliderMargin * 2;
			}
		}

		public PathGenStrategy()
		{
			DungeonGenAsset = Resources.Load<DungeonGenAsset>("Assets/DungeonGenAsset");
			Asset = Resources.Load<OnBorderPathGenAsset>("Assets/OnBorderPathGenAsset");
		}

        public abstract void ConnectRooms(MapData map);
    }
}