using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public StageData savedStage;
    public List<TowerPositionData> TowerData;
    public int currentRoundIndex = 0;

    public void AddData(TowerController tower, int x, int y)
    {
        if (TowerData == null)
            TowerData = new List<TowerPositionData>();

        savedStage = PlayerChoices.SelectedStage;
        TowerPositionData data = new TowerPositionData(tower, x, y);
        TowerData.Add(data);
    }

    public LevelSaveData(int roundIndex)
    {
        currentRoundIndex = roundIndex;
    }
}
