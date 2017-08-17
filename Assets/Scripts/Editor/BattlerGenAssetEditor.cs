﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameParameterAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create GameParameter Asset Instance")]
	public static void CreateGameParameterAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.GameParameterAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/GameParameter.asset");
		AssetDatabase.Refresh();
	}
}
