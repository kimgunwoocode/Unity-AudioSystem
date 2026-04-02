using UnityEditor;
using UnityEngine;

public class CreatAudioManagerObject
{
    [MenuItem("GameObject/Audio/AudioManager", false, 10)]
    static void CreateMyPrefab(MenuCommand menuCommand)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/AudioSystem/Prefab/AudioManager.prefab"
        );

        if (prefab == null)
        {
            Debug.LogError("AudioManager 프리팹을 찾을 수 없음");
            return;
        }


        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);

        Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);

        Selection.activeObject = instance;
    }
}