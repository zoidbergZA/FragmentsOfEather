using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateShipProperties
{
    [MenuItem("Assets/Create/Ship Properties")]
    public static void CreateMyAsset()
    {
        ShipProperties asset = ScriptableObject.CreateInstance<ShipProperties>();

        AssetDatabase.CreateAsset(asset, "Assets/Ship Properties/New ShipProperties.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
