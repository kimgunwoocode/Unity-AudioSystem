using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    public AudioData()
    {
        volume = 1f;
        pitch = 1f;
    }
}

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Audio/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    public List<AudioData> bgmList = new();
    public List<AudioData> sfxList = new();
}