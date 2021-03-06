﻿using UnityEngine;
using System.Collections;

namespace ProcedualLevels.Models
{
    public class DungeonGenAsset : ScriptableObject
	{
		[Tooltip("部屋の属する区画に対する余白サイズ")]
		[SerializeField]
        public int MarginSize = 4;
		[Tooltip("水平な通路の幅")]
		[SerializeField]
		public int HorizontalPathThickness = 1;
		[Tooltip("鉛直な通路の幅")]
		[SerializeField]
		public int VerticalPathThickness = 1;
		[SerializeField]
        public int WorldWidth = 640;
		[SerializeField]
        public int WorldHeight = 480;
		[Tooltip("一部屋あたりの、通路の削除を試みる回数")]
		[SerializeField]
        public int PathReducingChance = 4;
        [Tooltip("一部屋に対する敵のいるマスの割合")]
        public float EnemyCountRatio = 0.01f;
        [Tooltip("当たり判定用に余裕を取る大きさ")]
        public int ColliderMargin;
        public int PlatformSpan = 2;
    }
}
