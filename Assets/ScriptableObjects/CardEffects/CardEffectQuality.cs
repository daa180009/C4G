using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

/// <summary>
/// Used to determine if an object is allowed to be targetted by an effect
/// </summary>
[System.Serializable]
public abstract class CardEffectQuality
{
    public abstract Card.TargetType TargetType { get; }

    public virtual bool CheckQuality(TileController tile, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(TowerController tower, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(EnemyController enemy, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }
    public virtual bool CheckQuality(CardController card, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return false;
    }

    public abstract string GetDescription(WorldInfo worldInfo, bool isPlural);

#if UNITY_EDITOR
    /// <summary>
    /// Method called in CardGenerator window to display the information the
    /// script needs for customization
    /// </summary>
    public abstract void InputGUI();

    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Quality")]
    public static void MakeEmptyQuality()
    {
        string[] result = AssetDatabase.FindAssets("Quality_", new string[] { "Assets/ScriptableObjects/CardEffects/Samples" });
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardQualities/Quality_.cs";

            if (!AssetDatabase.CopyAsset(path, newPath))
                Debug.LogWarning($"Failed to copy {path}");
        }
    }

    /// <summary>
    /// Method called in CardGenerator window to display the information the
    /// script needs for customization
    /// </summary>
    public void OnInputGUI()
    {
        EditorGUI.indentLevel += 1;
        InputGUI();
        EditorGUI.indentLevel -= 1;
    }

    static public void LoadQuality(ref MonoScript qualityScript, ref CardEffectQuality quality, Card.TargetType targetType, string label = "Quality")
    {
        string targetTypeString = "";
        switch (targetType)
        {
            case Card.TargetType.Tiles: targetTypeString = "Tiles_"; break;
            case Card.TargetType.Enemies: targetTypeString = "Enemies_"; break;
            case Card.TargetType.Towers: targetTypeString = "Towers_"; break;
            case Card.TargetType.Cards: targetTypeString = "Cards_"; break;
            case Card.TargetType.None: return;
        }

        string[] foundAssets = AssetDatabase.FindAssets(targetTypeString + " t:monoscript", new string[] { "Assets/ScriptableObjects/CardEffects/CardQualities" });
        Array.Sort(foundAssets, (string a, string b) =>
        {
            string path = AssetDatabase.GUIDToAssetPath(a).Split('/').Last();
            return (path.Substring(targetTypeString.Length) == "NoQuality.cs") ? -1 : 1;
        });

        List<string> qualityScriptPaths = new List<string>();
        List<string> qualityScriptNames = new List<string>();
        List<int> qualityScriptOptions = new List<int>();
        int lastQualityScriptNumber = -1;

        for (int i = 0; i < foundAssets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
            qualityScriptPaths.Add(path);
            qualityScriptOptions.Add(i);

            string name = path.Split('/').Last();
            name = name.Substring(targetTypeString.Length, name.Length - 3 - targetTypeString.Length).AddSpacesToSentence();
            qualityScriptNames.Add(name);

            if (AssetDatabase.GetAssetPath(qualityScript) == path)
                lastQualityScriptNumber = i;
        }

        int qualityScriptNumber = (int)EditorGUILayout.IntPopup(label , lastQualityScriptNumber == -1 ? 0 : lastQualityScriptNumber, qualityScriptNames.ToArray(), qualityScriptOptions.ToArray());

        if (lastQualityScriptNumber != qualityScriptNumber)
        {
            qualityScript = (MonoScript)AssetDatabase.LoadAssetAtPath(qualityScriptPaths[qualityScriptNumber], typeof(MonoScript));
            quality = (CardEffectQuality)Activator.CreateInstance(qualityScript.GetClass());
        }
    }
#endif
}
