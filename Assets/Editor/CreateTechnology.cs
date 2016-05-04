using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateTechnology
{
    [MenuItem("Assets/Create/Technology upgrade")]
    public static void CreateMyAsset()
    {
        TechnologyUpgrade asset = ScriptableObject.CreateInstance<TechnologyUpgrade>();

        AssetDatabase.CreateAsset(asset, "Assets/Technologies/New Tech Upgrade.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
