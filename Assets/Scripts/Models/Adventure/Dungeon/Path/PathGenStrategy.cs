using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// 通路を生成するアルゴリズムを提供するクラスの抽象クラス。
    /// </summary>
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

        /// <summary>
        /// 部屋を通路で接続します。
        /// </summary>
        /// <returns>生成した接続のコレクション。</returns>
        /// <param name="map">接続すべき部屋の情報を提供するマップデータ。</param>
        public abstract IEnumerable<MapConnection> ConnectRooms(MapData map);
    }
}