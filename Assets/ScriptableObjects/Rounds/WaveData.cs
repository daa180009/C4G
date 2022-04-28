using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[System.Serializable]
public class WaveData 
{
    public List<WaveEnemyData> Enemies;

    public WaveData()
    {
        Enemies = new List<WaveEnemyData>();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Called in Round Generator to generate the editor GUI for editing this data
    /// </summary>
    public void OnInputGUI(int waveIndex)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Wave " + (1 + waveIndex), EditorStyles.boldLabel);
        GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Add Enemy", EditorStyles.miniButtonLeft))
            Enemies.Add(new WaveEnemyData());

        if (Enemies.Count == 0)
            GUI.backgroundColor = Color.white;
        else
            GUI.backgroundColor = Color.grey;
        if (GUILayout.Button("Remove Enemy", EditorStyles.miniButtonRight) && Enemies.Count > 0)
            Enemies.RemoveAt(Enemies.Count - 1);
        EditorGUILayout.EndHorizontal();

        string[] foundAssets = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Prefabs/Enemy" });
        /*Array.Sort(foundAssets, (string a, string b) =>
        {
            return (AssetDatabase.GUIDToAssetPath(a).Split('/').Last() == "NoPredicate.cs") ? -1 : 1;
        });*/

        List<string> assetPaths = new List<string>();
        List<string> assetNames = new List<string>();
        List<int> assetOptions = new List<int>();

        for (int i = 0; i < foundAssets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
            assetPaths.Add(path);
            assetOptions.Add(i);

            string name = path.Split('/').Last();
            name = name.Substring(0, name.Length - 7).AddSpacesToSentence();
            assetNames.Add(name);
        }

        for(int i = 0; i < Enemies.Count; i++)
            Enemies[i].OnInputGUI(i, assetPaths, assetNames, assetOptions);

    }
#endif
}
