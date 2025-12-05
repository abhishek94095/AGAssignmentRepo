using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private bool SoundEnabled = true;
    [SerializeField] private SoundConfigSO SoundConfig;

    public void Play(SoundType soundType)
    {
        if (!SoundEnabled || SoundConfig == null)
        {
            return;
        }

        SoundData soundData = SoundConfig.GetSound(soundType);
        if (soundData == null || soundData.Clip == null)
        {
            return;
        }

        PlayAudioClipWithoutStopping(soundData.Clip, soundData.Volume);
    }

    public void SetSoundEnabled(bool enabled)
    {
        SoundEnabled = enabled;
    }

    public void ToggleSound()
    {
        SoundEnabled = !SoundEnabled;
    }

    private void PlayAudioClipWithoutStopping(AudioClip audioClip, float volumeModifier = 1f)
    {
        if (!SoundEnabled || audioClip == null)
        {
            return;
        }

        GameObject temporaryAudioObject = new GameObject("TempAudio_" + audioClip.name);
        AudioSource audioSource = temporaryAudioObject.AddComponent<AudioSource>();

        audioSource.volume *= volumeModifier;
        audioSource.clip = audioClip;
        audioSource.Play();

        Destroy(temporaryAudioObject, audioClip.length);
    }
}

public enum SoundType
{
    Flip = 10,
    Match = 20,
    Mismatch = 30,
    GameOver = 40
}

[Serializable]
public class SoundData
{
    public SoundType Type;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume = 1f;
}
