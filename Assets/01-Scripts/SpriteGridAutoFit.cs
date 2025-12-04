using System.Collections.Generic;
using UnityEngine;

public class SpriteGridAutoFit : MonoBehaviour
{
    [Header("Grid Size")]
    [Min(1)] public int rows = 2;
    [Min(1)] public int columns = 2;

    [Header("Card Prefab")]
    public GameObject cardPrefab;

    [Header("Camera")]
    public Camera targetCamera;

    [Header("Card Layout")]
    public float cardSize = 1f;
    public float spacingX = 0.1f;
    public float spacingY = 0.1f;

    [Header("Grid Margin")]
    public float marginX = 0.5f;
    public float marginY = 0.5f;

    [Header("Card Assets")]
    public Sprite backSprite;
    public CardSpriteHandlerSO cardSpriteDataSO;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (cardSpriteDataSO != null)
            cardSpriteDataSO.Initialize();
    }

    public void DestroyGameObjects()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        if (cardPrefab == null || cardSpriteDataSO == null)
        {
            Debug.LogError("Missing cardPrefab or CardSpriteHandlerSO");
            return;
        }

        DestroyGameObjects();

        float totalWidth = columns * cardSize + (columns - 1) * spacingX;
        float totalHeight = rows * cardSize + (rows - 1) * spacingY;

        float reqHeight = totalHeight / 2f + marginY;
        float reqHeightFromWidth = (totalWidth / (2f * targetCamera.aspect)) + marginX;
        targetCamera.orthographicSize = Mathf.Max(reqHeight, reqHeightFromWidth);

        Vector3 center = targetCamera.transform.position;
        center.z = 0;

        float startX = center.x - totalWidth / 2f + cardSize / 2f;
        float startY = center.y + totalHeight / 2f - cardSize / 2f;

        SpriteRenderer sr = cardPrefab.GetComponent<SpriteRenderer>() ?? cardPrefab.GetComponentInChildren<SpriteRenderer>();
        float scaleFactor = cardSize / Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);

        int totalSlots = rows * columns;
        bool isEven = (rows * columns) % 2 == 0;
        if (!isEven)
        {
            // one slot will be skipped
            totalSlots -= 1;
        }

        // === NEW: generate shuffled pair IDs ===
        List<int> cardIds = GeneratePairedRandomIds(totalSlots);
        int index = 0;

        // reset game state
        if (MemoryGameController.Instance != null)
            MemoryGameController.Instance.ResetState();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (!isEven && r == rows - 1 && c == columns - 1)
                {
                    // Skip center card for odd count grids
                    continue;
                }

                Vector3 pos = new(startX + c * (cardSize + spacingX), startY - r * (cardSize + spacingY), 0);
                GameObject cardObj = Instantiate(cardPrefab, pos, Quaternion.identity, transform);

                // cardObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                CardView view = cardObj.GetComponent<CardView>();

                int id = cardIds[index++];
                view.Initialize(id, cardSpriteDataSO.GetSprite(id));

                // === NEW: register with game controller ===
                if (MemoryGameController.Instance != null)
                    MemoryGameController.Instance.RegisterCard(view);
            }
        }
    }

    // === NEW helper for random pair IDs ===
    private List<int> GeneratePairedRandomIds(int totalCards)
    {
        List<int> ids = new List<int>(totalCards);
        int pairCount = totalCards / 2;

        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Fisher–Yates shuffle
        for (int i = 0; i < ids.Count; i++)
        {
            int j = Random.Range(i, ids.Count);
            int temp = ids[i];
            ids[i] = ids[j];
            ids[j] = temp;
        }

        return ids;
    }
}
