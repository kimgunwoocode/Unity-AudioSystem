using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioDatabase))]
public class AudioDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Sound Keys"))
        {
            AudioKeyGenerator.Generate();
        }
    }
}