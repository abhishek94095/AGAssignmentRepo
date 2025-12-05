using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int GridId;
    public int Rows;
    public int Columns;
    public int TurnCount;
    public int MatchCount;
    public List<int> CardIdList;
}

public static class GameSaveManager
{
    private const string SaveKey = "SAVE_DATA_KEY";

    private static GameSaveData currentSaveData;

    public static GameSaveData CurrentSaveData => currentSaveData;
    public static bool HasSave => PlayerPrefs.HasKey(SaveKey);

    public static bool TryLoad(out GameSaveData gameSaveData)
    {
        if (currentSaveData != null)
        {
            gameSaveData = currentSaveData;
            return true;
        }

        if (!PlayerPrefs.HasKey(SaveKey))
        {
            gameSaveData = null;
            return false;
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            gameSaveData = null;
            return false;
        }

        gameSaveData = JsonUtility.FromJson<GameSaveData>(json);
        currentSaveData = gameSaveData;
        return gameSaveData != null;
    }

    public static void StartNewGame(int gridId, int rows, int columns, List<int> cardIdList)
    {
        currentSaveData = new GameSaveData
        {
            GridId = gridId,
            Rows = rows,
            Columns = columns,
            TurnCount = 0,
            MatchCount = 0,
            CardIdList = new List<int>(cardIdList)
        };

        SaveInternal();
    }

    public static void UpdateScore(int turnCount, int matchCount)
    {
        if (currentSaveData == null)
        {
            return;
        }

        currentSaveData.TurnCount = turnCount;
        currentSaveData.MatchCount = matchCount;
        SaveInternal();
    }

    public static void MarkCardDestroyed(int cardIndex)
    {
        if (currentSaveData == null)
        {
            return;
        }

        if (currentSaveData.CardIdList == null)
        {
            return;
        }

        if (cardIndex < 0 || cardIndex >= currentSaveData.CardIdList.Count)
        {
            return;
        }

        currentSaveData.CardIdList[cardIndex] = -1;
        SaveInternal();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        currentSaveData = null;
    }

    private static void SaveInternal()
    {
        if (currentSaveData == null)
        {
            return;
        }

        string json = JsonUtility.ToJson(currentSaveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
}
