using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualLevels.Models
{
    /// <summary>
    /// モデルでよく使われるアセットを管理するシングルトン クラス。
    /// </summary>
    public class AssetRepository
	{
		public static readonly string DungeonGenAssetPath = "Assets/DungeonGenAsset";
		public static readonly string GameParameterAssetPath = "Assets/GameParameterAsset";

        private static AssetRepository instance_;

        public static AssetRepository I
        {
            get { return instance_ = instance_ ?? new AssetRepository(); }
        }

        public DungeonGenAsset DungeonGenAsset { get; set; }
        public GameParameterAsset GameParameterAsset { get; set; }

        public AssetRepository()
        {
            DungeonGenAsset = Resources.Load<DungeonGenAsset>(DungeonGenAssetPath);
            GameParameterAsset = Resources.Load<GameParameterAsset>(GameParameterAssetPath);
        }
    }
}