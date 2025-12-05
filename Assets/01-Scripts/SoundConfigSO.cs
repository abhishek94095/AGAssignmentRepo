using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "SoundConfig", menuName = "Audio/Sound Config")]
public class SoundConfigSO : ScriptableObject
{
    [SerializeField] private List<SoundData> Sounds = new List<SoundData>();

    private Dictionary<SoundType, SoundData> soundLookupDictionary;

    private void OnEnable()
    {
        BuildLookup();
    }

    public SoundData GetSound(SoundType soundType)
    {
        if (soundLookupDictionary == null || soundLookupDictionary.Count == 0)
        {
            BuildLookup();
        }

        soundLookupDictionary.TryGetValue(soundType, out SoundData soundData);
        return soundData;
    }

    private void BuildLookup()
    {
        soundLookupDictionary = new Dictionary<SoundType, SoundData>();

        for (int i = 0; i < Sounds.Count; i++)
        {
            SoundData soundData = Sounds[i];

            if (soundData == null || soundData.Clip == null)
            {
                continue;
            }

            if (!soundLookupDictionary.ContainsKey(soundData.Type))
            {
                soundLookupDictionary.Add(soundData.Type, soundData);
            }
        }
    }
}
