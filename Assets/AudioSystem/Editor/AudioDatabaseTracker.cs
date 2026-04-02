using UnityEditor;
using UnityEngine;

public class AudioDatabaseTracker : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var config = AudioConfigProvider.GetOrCreate();
        if (config == null) return;

        bool changed = false;


        foreach (var path in importedAssets)
        {
            var db = AssetDatabase.LoadAssetAtPath<AudioDatabase>(path);
            if (db != null && !config.audioDatabases.Contains(db))
            {
                config.audioDatabases.Add(db);
                changed = true;
            }
        }


        foreach (var path in movedAssets)
        {
            var db = AssetDatabase.LoadAssetAtPath<AudioDatabase>(path);
            if (db != null && !config.audioDatabases.Contains(db))
            {
                config.audioDatabases.Add(db);
                changed = true;
            }
        }


        foreach (var path in deletedAssets)
        {
            for (int i = config.audioDatabases.Count - 1; i >= 0; i--)
            {
                var db = config.audioDatabases[i];

                if (db == null)
                {
                    config.audioDatabases.RemoveAt(i);
                    changed = true;
                    continue;
                }

                string dbPath = AssetDatabase.GetAssetPath(db);

                if (dbPath == path)
                {
                    config.audioDatabases.RemoveAt(i);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}