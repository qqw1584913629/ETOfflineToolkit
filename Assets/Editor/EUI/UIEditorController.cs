using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIEditorController
{
    [MenuItem("GameObject/SpawnEUICode", false, -2)]
    static public void CreateNewCode()
    {
        GameObject go = Selection.activeObject as GameObject;
        UICodeSpawner.SpawnEUICode(go);
    }


    [MenuItem("ET/EUICodeSpawn", false, 1007)]
    static public void CreateAllEUICode()
    {
        string folderPath = "Assets/Bundles/UI/Dlg/";
        CreateUICodeByPath(folderPath);
        folderPath = "Assets/Bundles/UI/Item/";
        CreateUICodeByPath(folderPath);
        folderPath = "Assets/Bundles/UI/Common/";
        CreateUICodeByPath(folderPath);
    }

    private static void CreateUICodeByPath(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            if (prefab != null)
            {
                UICodeSpawner.SpawnEUICode(prefab);
                Debug.Log($"生成{prefab.name}的UI代码");
            }
        }
    }
}
