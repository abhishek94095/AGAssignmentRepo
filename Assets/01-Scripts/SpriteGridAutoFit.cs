using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteGridAutoFit : MonoBehaviour
{
    [SerializeField] private int Rows = 2;
    [SerializeField] private int Columns = 2;

    [SerializeField] private CardView CardPrefab;
    [SerializeField] private Camera TargetCamera;

    [SerializeField] private float CardSize = 1f;
    [SerializeField] private float SpacingX = 0.1f;
    [SerializeField] private float SpacingY = 0.1f;

    [SerializeField] private float MarginX = 0.5f;
    [SerializeField] private float MarginY = 0.5f;

    [SerializeField] private CardSpriteHandlerSO CardSpriteDatabase;

    [SerializeField] private int CurrentGridId;

    private void Start()
    {
        if (TargetCamera == null)
        {
            TargetCamera = Camera.main;
        }

        if (CardSpriteDatabase != null)
        {
            CardSpriteDatabase.Initialize();
        }
    }

    public void ConfigureGrid(int rows, int columns, int gridId)
    {
        Rows = rows;
        Columns = columns;
        CurrentGridId = gridId;
        RebuildGrid();
    }

    public void DestroyGridObjects()
    {
        for (int childIndex = transform.childCount - 1; childIndex >= 0; childIndex--)
        {
            Transform childTransform = transform.GetChild(childIndex);
            Destroy(childTransform.gameObject);
        }
    }

    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        if (CardPrefab == null || CardSpriteDatabase == null)
        {
            return;
        }

        DestroyGridObjects();

        float totalWidth = Columns * CardSize + (Columns - 1) * SpacingX;
        float totalHeight = Rows * CardSize + (Rows - 1) * SpacingY;

        float requiredHeight = totalHeight / 2f + MarginY;
        float requiredHeightFromWidth = totalWidth / (2f * TargetCamera.aspect) + MarginX;
        TargetCamera.orthographicSize = Mathf.Max(requiredHeight, requiredHeightFromWidth);

        Vector3 cameraCenter = TargetCamera.transform.position;
        cameraCenter.z = 0f;

        float startX = cameraCenter.x - totalWidth / 2f + CardSize / 2f;
        float startY = cameraCenter.y + totalHeight / 2f - CardSize / 2f;

        SpriteRenderer spriteRenderer = CardPrefab.GetComponent<SpriteRenderer>() ?? CardPrefab.GetComponentInChildren<SpriteRenderer>();
        float scaleFactor = CardSize / Mathf.Max(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);

        int totalSlots = Rows * Columns;
        bool isEvenSlotCount = totalSlots % 2 == 0;

        if (!isEvenSlotCount)
        {
            totalSlots -= 1;
        }

        List<int> cardIdList;
        GameSaveData saveData;

        bool hasValidSave =
            GameSaveManager.TryLoad(out saveData) &&
            saveData.GridId == CurrentGridId &&
            saveData.Rows == Rows &&
            saveData.Columns == Columns &&
            saveData.CardIdList != null &&
            saveData.CardIdList.Count == totalSlots;

        if (hasValidSave)
        {
            cardIdList = new List<int>(saveData.CardIdList);
        }
        else
        {
            cardIdList = GeneratePairedRandomIds(totalSlots);
            GameSaveManager.StartNewGame(CurrentGridId, Rows, Columns, cardIdList);
        }

        List<CardView> createdCards = new List<CardView>();
        int cardListIndex = 0;

        MemoryGameController.Instance.ResetState();

        for (int rowIndex = 0; rowIndex < Rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < Columns; columnIndex++)
            {
                if (!isEvenSlotCount && rowIndex == Rows - 1 && columnIndex == Columns - 1)
                {
                    continue;
                }

                int cardId = cardIdList[cardListIndex];

                if (cardId == -1)
                {
                    cardListIndex++;
                    continue;
                }

                Vector3 position = new Vector3(
                    startX + columnIndex * (CardSize + SpacingX),
                    startY - rowIndex * (CardSize + SpacingY),
                    0f);

                CardView cardInstance = Instantiate(CardPrefab, position, Quaternion.identity, transform);

                cardInstance.SetCardIndex(cardListIndex);
                cardInstance.Initialize(cardId, CardSpriteDatabase.GetSprite(cardId));

                createdCards.Add(cardInstance);
                MemoryGameController.Instance.RegisterCard(cardInstance);

                cardListIndex++;
            }
        }

        RevealCardsAtStart(createdCards);
    }

    private void RevealCardsAtStart(List<CardView> cardList)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            CardView cardView = cardList[i];
            if (cardView == null)
            {
                continue;
            }

            cardView.Flip(true);
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                CardView cardView = cardList[i];
                if (cardView == null)
                {
                    continue;
                }

                cardView.Flip(true);
            }
        });
    }

    private List<int> GeneratePairedRandomIds(int totalCardCount)
    {
        List<int> cardIdList = new List<int>(totalCardCount);
        int pairCount = totalCardCount / 2;

        for (int pairIndex = 0; pairIndex < pairCount; pairIndex++)
        {
            cardIdList.Add(pairIndex);
            cardIdList.Add(pairIndex);
        }

        for (int i = 0; i < cardIdList.Count; i++)
        {
            int shuffleIndex = Random.Range(i, cardIdList.Count);
            int temporaryCardId = cardIdList[i];
            cardIdList[i] = cardIdList[shuffleIndex];
            cardIdList[shuffleIndex] = temporaryCardId;
        }

        return cardIdList;
    }
}
