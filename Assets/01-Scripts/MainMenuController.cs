using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private List<Button> GridButtons;
    [SerializeField] private GridUIController GridController;
    [SerializeField] private TextMeshProUGUI TurnText;
    [SerializeField] private TextMeshProUGUI MatchText;
    [SerializeField] private GameObject GameplayRoot;
    [SerializeField] private GameObject MainMenuRoot;
    [SerializeField] private GameObject ResumeButtonRoot;

    private int turnCount;
    private int matchCount;

    private void Start()
    {
        InitializeGridButtons();
        MemoryGameController.TurnCompleted += HandleTurnCompleted;
        MemoryGameController.GameCompleted += HandleGameCompleted;
        UpdateResumeButtonState();
    }

    public void HandleHomeButtonClicked()
    {
        GameplayRoot.SetActive(false);
        MainMenuRoot.SetActive(true);

        GridController.DestroyCurrentGrid();
        UpdateResumeButtonState();
    }

    public void HandleResumeButtonClicked()
    {
        if (!GameSaveManager.TryLoad(out GameSaveData saveData))
        {
            return;
        }

        GridController.SetGrid(saveData.GridId);

        turnCount = saveData.TurnCount;
        matchCount = saveData.MatchCount;
        ApplyScoreToUi();

        MainMenuRoot.SetActive(false);
        GameplayRoot.SetActive(true);
    }

    private void InitializeGridButtons()
    {
        GridPresetConfig[] presets = GridController.CurrentGridPresents;

        int buttonCount = Mathf.Min(GridButtons.Count, presets.Length);

        for (int i = 0; i < buttonCount; i++)
        {
            GridPresetConfig preset = presets[i];
            Button gridButton = GridButtons[i];

            int presetId = preset.Id;

            gridButton.onClick.RemoveAllListeners();
            gridButton.onClick.AddListener(() => HandleGridSelected(presetId));

            TextMeshProUGUI label = gridButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            label.text = $"{preset.Rows} x {preset.Columns}";

            gridButton.gameObject.name = $"{preset.Rows}x{preset.Columns}";
        }
    }

    private void HandleGridSelected(int presetId)
    {
        GameSaveManager.Clear();
        GridController.SetGrid(presetId);

        GameplayRoot.SetActive(true);
        MainMenuRoot.SetActive(false);

        turnCount = 0;
        matchCount = 0;
        ApplyScoreToUi();
    }

    private void HandleTurnCompleted(bool isMatch)
    {
        turnCount++;

        if (isMatch)
        {
            matchCount++;
        }

        ApplyScoreToUi();
        GameSaveManager.UpdateScore(turnCount, matchCount);
    }

    private void HandleGameCompleted()
    {
        GameplayRoot.SetActive(false);
        MainMenuRoot.SetActive(true);

        GridController.DestroyCurrentGrid();
        GameSaveManager.Clear();
        UpdateResumeButtonState();
    }

    private void ApplyScoreToUi()
    {
        TurnText.text = turnCount.ToString();
        MatchText.text = matchCount.ToString();
    }

    private void UpdateResumeButtonState()
    {
        if (ResumeButtonRoot != null)
        {
            ResumeButtonRoot.SetActive(GameSaveManager.HasSave);
        }
    }
}
