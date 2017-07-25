﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattlerGenAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create BattlerGen Asset Instance")]
	public static void CreateBattlerGenAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.BattlerGenAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Asset/BattlerGenAsset.asset");
		AssetDatabase.Refresh();
	}
}
