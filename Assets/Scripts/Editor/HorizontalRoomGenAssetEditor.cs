﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HorizontalRoomGenAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create HorizontalRoomGen Asset Instance")]
	public static void CreateHorizontalRoomGenAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.HorizontalRoomGenAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/HorizontalRoomGenAsset.asset");
		AssetDatabase.Refresh();
	}
}
