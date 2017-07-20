﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonGenAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create DungeonGen Asset Instance")]
	public static void CreateDungeonGenAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.DungeonGenAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Asset/DungeonGenAsset.asset");
		AssetDatabase.Refresh();
	}
}
