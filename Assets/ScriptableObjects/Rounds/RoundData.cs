using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RoundData : ScriptableObject
{
    public List<WaveData> EnemyWaves = new List<WaveData>();

#if UNITY_EDITOR
    [MenuItem("Assets/Create/ScriptableObjects/Round Data", false, 20)]
    public static void CreateRoundData()
    {
        RoundData data = ScriptableObject.CreateInstance<RoundData>();

        AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Round/_.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = data;
    }

    /// <summary>
    /// Called in Round Generator to generate the editor GUI for editing this data
    /// </summary>
    public void OnInputGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Enemy Wave Configuration");
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Add Wave", EditorStyles.miniButtonLeft))
            EnemyWaves.Add(new WaveData());

        if (EnemyWaves.Count == 0)
            GUI.backgroundColor = Color.white;
        else
            GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Remove Wave", EditorStyles.miniButtonRight) && EnemyWaves.Count > 0)
            EnemyWaves.RemoveAt(EnemyWaves.Count - 1);
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < EnemyWaves.Count; i++)
        {
            EnemyWaves[i].OnInputGUI(i);
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
#endif
}