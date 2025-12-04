using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<Button> gridButtons;
    public GridUIController gridController;
    public TextMeshProUGUI turnsText, matchesText;
    public GameObject gameplayMenu, mainMenu;
    private int turnsCount = 0, matchesCount = 0;

    private void Start()
    {
        for (int i = 0; i < gridButtons.Count; i++)
        {
            int index = i + 1; // Capture index for the closure
            gridButtons[i].onClick.AddListener(() => OnGridButtonClicked(index));
            gridButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{gridController.configs[i].rows} x {gridController.configs[i].columns}";
            gridButtons[i].gameObject.name = $"{gridController.configs[i].rows}x{gridController.configs[i].columns}";
        }
        MemoryGameController.onTurnCompleted += (isMatch) =>
        {
            UpdateUI(isMatch);
        };
    }

    private void OnGridButtonClicked(int index)
    {
        gridController.SetGrid(index);
        gameplayMenu.SetActive(true);
        mainMenu.SetActive(false);
        turnsCount = 0;
        matchesCount = 0;
    }

    public void OnHomeButtonClicked()
    {
        gameplayMenu.SetActive(false);
        mainMenu.SetActive(true);
        gridController.grid.DestroyGameObjects();
    }

    public void UpdateUI(bool isMatch)
    {
        turnsCount++;
        if (isMatch)
            matchesCount++;

        turnsText.text = $"{turnsCount}";
        matchesText.text = $"{matchesCount}";
    }
}
