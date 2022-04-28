using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

/// <summary>
/// Class that card effect conditions inherit from.
/// A condition is a script that is run when trying to resolve a 
/// card effect to determine if that effect should happen.
/// </summary>
[System.Serializable]
public abstract class CardEffectCondition
{
    /// <summary>
    /// Method called during runtime to actually check if the condition
    /// is true (and whether to run the effect preedicate)
    /// </summary>
    public abstract bool CheckCondition(WorldInfo worldInfo, ResolutionInfo resolutionInfo);

    public abstract string GetDescription(WorldInfo worldInfo);

#if UNITY_EDITOR
    /// <summary>
    /// Called to set up GUI for class-specific customization
    /// </summary>
    public abstract void InputGUI();

    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Condition")]
    public static void MakeEmptyCondition()
    {
        string[] result = AssetDatabase.FindAssets("Condition_", new string[] { "Assets/ScriptableObjects/CardEffects/Samples" });
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardConditions/Condition_.cs";

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

    static public void LoadCondition(ref MonoScript conditionScript, ref CardEffectCondition condition, string label = "Condition")
    {
        string[] foundAssets = AssetDatabase.FindAssets("", new string[] { "Assets/ScriptableObjects/CardEffects/CardConditions" });
        Array.Sort(foundAssets, (string a, string b) =>
        {
            return (AssetDatabase.GUIDToAssetPath(a).Split('/').Last() == "NoCondition.cs") ? -1 : 1;
        });

        List<string> conditionScriptPaths = new List<string>();
        List<string> conditionScriptNames = new List<string>();
        List<int> conditionScriptOptions = new List<int>();
        int lastConditionScriptNumber = -1;

        for (int i = 0; i < foundAssets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
            conditionScriptPaths.Add(path);
            conditionScriptOptions.Add(i);

            string name = path.Split('/').Last();
            name = name.Substring(0, name.Length - 3).AddSpacesToSentence();
            conditionScriptNames.Add(name);

            if (AssetDatabase.GetAssetPath(conditionScript) == path)
                lastConditionScriptNumber = i;
        }

        int conditionScriptNumber = (int)EditorGUILayout.IntPopup(label, lastConditionScriptNumber == -1 ? 0 : lastConditionScriptNumber, conditionScriptNames.ToArray(), conditionScriptOptions.ToArray());

        if (lastConditionScriptNumber != conditionScriptNumber)
        {
            conditionScript = (MonoScript)AssetDatabase.LoadAssetAtPath(conditionScriptPaths[conditionScriptNumber], typeof(MonoScript));
            condition = (CardEffectCondition)Activator.CreateInstance(conditionScript.GetClass());
        }
    }
#endif
}
