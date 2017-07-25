﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class $name$AssetEditor : ScriptableObject
{
	[MenuItem("Assets/Create/Game Asset/Create $name$ Asset Instance")]
	public static void Create$name$AssetInstance()
	{
		var asset = CreateInstance<ProcedualLevels.Models.$name$Asset>();
		AssetDatabase.CreateAsset(asset, "Assets/Resources/Assets/$name$Asset.asset");
		AssetDatabase.Refresh();
	}
}
