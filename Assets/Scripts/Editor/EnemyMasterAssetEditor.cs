﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyMasterAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create EnemyMaster Asset Instance")]
	public static void CreateEnemyMasterAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.EnemyMasterAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/EnemyMasterAsset.asset");
		AssetDatabase.Refresh();
	}
}
