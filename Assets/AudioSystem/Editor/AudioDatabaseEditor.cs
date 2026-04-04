using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioDatabase))]
public class AudioDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Audio System Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Audio Keys"))
        {
            AudioKeyGenerator.Generate();
        }
    }
}