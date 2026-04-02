using UnityEditor;
using UnityEngine;

public static class AudioConfigProvider
{
    private static AudioSystemConfig config;

    public static AudioSystemConfig GetOrCreate()
    {
        if (config != null) return config;

        var guids = AssetDatabase.FindAssets("t:AudioSystemConfig");

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            config = AssetDatabase.LoadAssetAtPath<AudioSystemConfig>(path);
        }
        else
        {
            config = ScriptableObject.CreateInstance<AudioSystemConfig>();
            AssetDatabase.CreateAsset(config, "Assets/AudioSystem/Data/AudioSystemConfig.asset");
            AssetDatabase.SaveAssets();
        }
        return config;
    }
}