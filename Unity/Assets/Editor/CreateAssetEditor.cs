using UnityEditor;
using UnityEngine;

public class CreateAssetEditor
{
    [MenuItem("Tools/Create/Android Config")]
    static void CreateScriptObject()
    {
        AndroidBuildConfigAsset createAsset = ScriptableObject.CreateInstance<AndroidBuildConfigAsset>();
        AssetDatabase.CreateAsset(createAsset, "Assets/Config/Build/AndroidBuildConfig.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}