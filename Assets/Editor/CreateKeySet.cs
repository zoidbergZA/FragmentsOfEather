using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateKeySet
{
    [MenuItem("Assets/Create/Input Key Set")]
    public static void CreateMyAsset()
    {
        KeySet asset = ScriptableObject.CreateInstance<KeySet>();

        AssetDatabase.CreateAsset(asset, "Assets/Key Sets/New KeySet.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
