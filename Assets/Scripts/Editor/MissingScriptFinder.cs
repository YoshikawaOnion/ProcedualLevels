using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class MissinScriptFinder : ScriptableObject
{
    [MenuItem("Tools/Onion.Yoshikawa/Check Missing Script")]
    public static void CheckMissingScript()
    {
        var result = new List<string>();
        var prefabPathes = GetAllPrefabs();
        foreach (var path in prefabPathes)
        {
            var scripts = LoadPrefabsScripts(path);
            foreach (var script in scripts)
            {
                if (script == null)
                {
                    result.Add(path);
                    Debug.LogWarning("プレハブ " + path + " は無効なコンポーネントがアタッチされています", script);
                }
            }
        }
        Debug.Log(result.Count + "個の無効なコンポーネントがアタッチされたプレハブが存在します");
    }

    private static Component[] LoadPrefabsScripts(string path)
    {
        var prefab = AssetDatabase.LoadMainAssetAtPath(path);
        GameObject gameobject;
        try
        {
            gameobject = (GameObject)prefab;
            return gameobject.GetComponentsInChildren<Component>(true).ToArray();
        }
        catch (System.Exception ex)
        {
            Debug.Log("プレハブ " + path + " は GameObject に変換できませんでした");
            return new Component[0];
        }
    }

    private static string[] GetAllPrefabs()
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        var result = new List<string>();
        foreach (var item in temp)
        {
            if (item.EndsWith(".prefab"))
            {
                result.Add(item);
            }
        }
        return result.ToArray();
    }
}
