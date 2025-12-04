using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardSpriteData
{
    public int id;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "CardSpriteHandler", menuName = "Card System/Card Sprite Handler")]
public class CardSpriteHandlerSO : ScriptableObject
{
    [Header("Default Fallback Sprite")]
    public Sprite defaultSprite;

    [Header("Sprite Mapping")]
    public List<CardSpriteData> sprites = new List<CardSpriteData>();
    // public List<Sprite> extraSprites = new List<Sprite>();

    private Dictionary<int, Sprite> lookup;

    /// <summary>
    /// Builds lookup table — should be called once.
    /// </summary>
    public void Initialize()
    {
        lookup = new Dictionary<int, Sprite>();

        foreach (var entry in sprites)
        {
            if (entry.sprite == null) continue;
            if (!lookup.ContainsKey(entry.id))
                lookup.Add(entry.id, entry.sprite);
        }
    }

    public Sprite GetSprite(int id)
    {
        if (lookup == null) Initialize();

        if (lookup.TryGetValue(id, out Sprite sprite))
            return sprite;

        return defaultSprite;
    }

    // [ContextMenu("Set Sprite From Data")]
    // public void SetSpriteFromData()
    // {
    //     int count = 0;
    //     sprites = new List<CardSpriteData>();
    //     foreach (var sprite in extraSprites)
    //     {
    //         if (sprite == null) continue;
    //         CardSpriteData data = new CardSpriteData
    //         {
    //             id = count++,
    //             sprite = sprite
    //         };
    //         sprites.Add(data);
    //     }
    // }
}
