using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardSpriteData
{
    public int Id;
    public Sprite Sprite;
}

// [CreateAssetMenu(fileName = "CardSpriteHandler", menuName = "Card System/Card Sprite Handler")]
public class CardSpriteHandlerSO : ScriptableObject
{
    [SerializeField] private Sprite DefaultSprite;
    [SerializeField] private List<CardSpriteData> SpriteDataList = new List<CardSpriteData>();
    private Dictionary<int, Sprite> SpriteLookupDictionary;

    public void Initialize()
    {
        SpriteLookupDictionary = new Dictionary<int, Sprite>();

        for (int i = 0; i < SpriteDataList.Count; i++)
        {
            CardSpriteData spriteData = SpriteDataList[i];
            if (spriteData.Sprite == null) continue;

            if (!SpriteLookupDictionary.ContainsKey(spriteData.Id))
            {
                SpriteLookupDictionary.Add(spriteData.Id, spriteData.Sprite);
            }
        }
    }

    public Sprite GetSprite(int spriteId)
    {
        if (SpriteLookupDictionary == null) Initialize();

        if (SpriteLookupDictionary.TryGetValue(spriteId, out Sprite sprite)) return sprite;

        return DefaultSprite;
    }
}
