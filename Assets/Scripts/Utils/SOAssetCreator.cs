using AI;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SOAssetCreator
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (String.Empty == path)
        {
            path = "Assets/Descriptors/";
        }
        else
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), String.Empty);
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("AI/Consideration Handler", false, 20)]
    public static void CreateConsiderationHandler()
    {
        CreateAsset<ConsiderationHandler>();
    }
}