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
    public int enemiesCount = 1;

    public int enemiesSpawned;

    public WaveEnemyData()
    {
    }

    public void Restart()
    {
        enemiesSpawned = 0;
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
        enemiesCount = (int)EditorGUILayout.IntField("#: ", enemiesCount);
        enemiesCount = Mathf.Clamp(enemiesCount, 1, 1000);
        EditorGUI.indentLevel -= 1;

        EditorGUI.indentLevel -= 1;
    }
#endif
}