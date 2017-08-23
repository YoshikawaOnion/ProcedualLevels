﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnerMasterAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create SpawnerMaster Asset Instance")]
	public static void CreateSpawnerMasterAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.SpawnerMasterAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/SpawnerMasterAsset.asset");
		AssetDatabase.Refresh();
	}
}
