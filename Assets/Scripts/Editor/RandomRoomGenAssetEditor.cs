﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RandomRoomGenAssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create RandomRoomGen Asset Instance")]
	public static void CreateRandomRoomGenAssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.RandomRoomGenAsset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/RandomRoomGenAsset.asset");
		AssetDatabase.Refresh();
	}
}
