using AudioSystem;
using TMPro;
using UnityEngine;

public class AudioManager_test : MonoBehaviour
{
    public TMP_Text activeSFXcount_text;
    public TMP_Text SFXqueuecount_text;

    public Transform obj;

    AudioHandle handle;


    private void Start()
    {
        // AudioManager.PlaySFX(AudioKeys.SFX.SampleSFX_01);
    }

    private void Update()
    {
        activeSFXcount_text.text = AudioManager.GetActiveSFXCount().ToString();
        SFXqueuecount_text.text = AudioManager.GetSFXpoolCount().ToString();
    }

    public void PlaySampleSFX()
    {
        handle = AudioManager.PlaySFX(AudioKeys.SFX.SampleSFX_01, obj.position, true);
    }

    public void StopSampleSFX()
    {
        AudioManager.StopSFX(handle);
    }
}