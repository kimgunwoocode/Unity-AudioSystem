using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AudioDatabaseRescan
{
    static AudioDatabaseRescan()
    {
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private static void OnUndoRedo()
    {
        Rescan();
    }

    [MenuItem("Tools/AudioSystem/Rescan Databases")]
    public static void Rescan()
    {
        var config = AudioConfigProvider.GetOrCreate();
        if (config == null) return;

        var guids = AssetDatabase.FindAssets("t:AudioDatabase");

        config.audioDatabases.Clear();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var db = AssetDatabase.LoadAssetAtPath<AudioDatabase>(path);

            if (db != null)
            {
                config.audioDatabases.Add(db);
            }
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log($"AudioDatabase Rescan 완료: {config.audioDatabases.Count}개 등록됨");
    }
}