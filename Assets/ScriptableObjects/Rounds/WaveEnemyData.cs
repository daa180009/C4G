using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[System.Serializable]
public class WaveEnemyData
{
    public GameObject prefab;
    public float enemiesCount = 1;
    public float increasePerRound = 1;

    public int enemiesSpawned;

    public WaveEnemyData()
    {
    }

    public void Restart()
    {
        enemiesSpawned = 0;
    }

    public int getCount(int roundNumber)
    {
        return Mathf.Clamp((int)Mathf.Floor(enemiesCount + increasePerRound * (float)roundNumber), 0, 1000);
    }

    public bool canSpawnMore(int roundNumber)
    {
        return enemiesSpawned < getCount(roundNumber);
    }

    public void enemySpawned()
    {
        enemiesSpawned += 1;
    }

#if UNITY_EDITOR
    public void OnInputGUI(int enemyIndex, List<string> assetPaths, List<string> assetNames, List<int> assetOptions)
    {
        EditorGUI.indentLevel += 1;

        // choose enemy type
        string currentPath = AssetDatabase.GetAssetPath(prefab);
        int currentID = assetPaths.FindIndex((string str) => str == currentPath);
        int newID = (int)EditorGUILayout.IntPopup("Enemy " + (enemyIndex + 1) + ":", currentID == -1 ? 0 : currentID, assetNames.ToArray(), assetOptions.ToArray());
        if (newID != currentID)
            prefab = (GameObject)AssetDatabase.LoadAssetAtPath(assetPaths[newID], typeof(GameObject));

        // choose enemy count
        EditorGUI.indentLevel += 1;
        enemiesCount = (float)EditorGUILayout.FloatField("Initial #: ", enemiesCount);
        enemiesCount = Mathf.Clamp(enemiesCount, -1000f, 1000f);
        increasePerRound = (float)EditorGUILayout.FloatField("Increase per round: ", increasePerRound);
        increasePerRound = Mathf.Clamp(increasePerRound, -1000f, 1000f);
        EditorGUI.indentLevel -= 1;

        EditorGUI.indentLevel -= 1;
    }
#endif
}