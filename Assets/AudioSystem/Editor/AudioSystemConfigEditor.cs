using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSystemConfig))]
public class AudioSystemConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Audio System Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Rescan Audio Databases"))
        {
            AudioDatabaseRescan.Rescan();
        }
    }
}