using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateLevel
{
    [MenuItem("Assets/Create/Level")]
    public static void CreateMyAsset()
    {
        Level asset = ScriptableObject.CreateInstance<Level>();

        AssetDatabase.CreateAsset(asset, "Assets/Levels/New Level.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
