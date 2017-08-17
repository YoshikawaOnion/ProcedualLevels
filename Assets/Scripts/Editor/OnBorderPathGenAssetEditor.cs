﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OnBorderPathGenAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create OnBorderPathGen Asset Instance")]
	public static void CreateOnBorderPathGenAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.OnBorderPathGenAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/OnBorderPathGenAsset.asset");
		AssetDatabase.Refresh();
	}
}
