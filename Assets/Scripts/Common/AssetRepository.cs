﻿using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UnityEngine;

namespace ProcedualLevels.Common
{
    /// <summary>
    /// モデルでよく使われるアセットを管理するシングルトン クラス。
    /// </summary>
    public class AssetRepository
	{
		public static readonly string DungeonGenAssetPath = "Assets/DungeonGenAsset";
		public static readonly string GameParameterAssetPath = "Assets/GameParameterAsset";
        public static readonly string EnemyMasterAssetPath = "Assets/EnemyMasterAsset";
        public static readonly string SpawnerMasterAssetPath = "Assets/SpawnerMasterAsset";

        private static AssetRepository instance_;

        public static AssetRepository I
        {
            get { return instance_ = instance_ ?? new AssetRepository(); }
        }

        public DungeonGenAsset DungeonGenAsset { get; private set; }
        public GameParameterAsset GameParameterAsset { get; private set; }
        public EnemyMasterAsset EnemyMaster { get; private set; }
        public SpawnerMasterAsset SpawnerMaster { get; private set; }

        public AssetRepository()
        {
            DungeonGenAsset = Resources.Load<DungeonGenAsset>(DungeonGenAssetPath);
            GameParameterAsset = Resources.Load<GameParameterAsset>(GameParameterAssetPath);
            EnemyMaster = Resources.Load<EnemyMasterAsset>(EnemyMasterAssetPath);
            SpawnerMaster = Resources.Load<SpawnerMasterAsset>(SpawnerMasterAssetPath);
        }
    }
}