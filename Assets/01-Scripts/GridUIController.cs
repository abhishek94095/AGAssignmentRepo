using UnityEngine;

[System.Serializable]
public struct GridPresetConfig
{
    public int Id;
    public int Rows;
    public int Columns;
}

public class GridUIController : MonoBehaviour
{
    [SerializeField] private SpriteGridAutoFit GridLayout;
    [SerializeField] private GridPresetConfig[] GridPresets;

    public GridPresetConfig[] CurrentGridPresents => GridPresets;

    public void SetGrid(int presetId)
    {
        for (int i = 0; i < GridPresets.Length; i++)
        {
            GridPresetConfig config = GridPresets[i];
            if (config.Id != presetId)
            {
                continue;
            }

            GridLayout.ConfigureGrid(config.Rows, config.Columns, config.Id);
            return;
        }
    }

    public void DestroyCurrentGrid()
    {
        GridLayout.DestroyGridObjects();
    }
}
