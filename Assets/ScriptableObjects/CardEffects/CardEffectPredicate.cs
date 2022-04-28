using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

/// <summary>
/// Class that card effect predicates inherit from.
/// A predicate is the actual effect of the card effect, such
/// as dealing damage to targets.
/// </summary>
[System.Serializable]
public abstract class CardEffectPredicate
{
    public abstract Card.TargetType TargetType { get; }

    /// <summary>
    /// Some predicates which target tiles can have an area of effect. If null, don't consider this.
    /// </summary>
    public AreaOfEffect AffectedArea;

    public abstract void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo);
    public abstract string GetDescription(WorldInfo worldInfo);

#if UNITY_EDITOR
    /// <summary>
    /// Called to set up GUI for class-specific customization
    /// </summary>
    public abstract void InputGUI();
    
    /// <summary>
    /// Used to easily duplicate a base class script for a new condition
    /// </summary>
    [MenuItem("Utilities/Card Effects/Make Empty Predicate")]
    public static void MakeEmptyPredicate()
    {
        string[] result = AssetDatabase.FindAssets("Predicate_", new string[] { "Assets/ScriptableObjects/CardEffects/Samples" });
        if (result.Length == 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(result[0]);
            string newPath = "Assets/ScriptableObjects/CardEffects/CardPredicates/Predicate_.cs";

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

    static public void LoadPredicate(ref MonoScript predicateScript, ref CardEffectPredicate predicate, string label = "Predicate")
    {
        string[] foundAssets = AssetDatabase.FindAssets("", new string[] { "Assets/ScriptableObjects/CardEffects/CardPredicates" });
        Array.Sort(foundAssets, (string a, string b) =>
        {
            return (AssetDatabase.GUIDToAssetPath(a).Split('/').Last() == "NoPredicate.cs") ? -1 : 1;
        });

        List<string> predicateScriptPaths = new List<string>();
        List<string> predicateScriptNames = new List<string>();
        List<int> predicateScriptOptions = new List<int>();
        int lastPredicateScriptNumber = -1;

        for (int i = 0; i < foundAssets.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
            predicateScriptPaths.Add(path);
            predicateScriptOptions.Add(i);

            string name = path.Split('/').Last();
            name = name.Substring(0, name.Length - 3).AddSpacesToSentence();
            predicateScriptNames.Add(name);

            if (AssetDatabase.GetAssetPath(predicateScript) == path)
                lastPredicateScriptNumber = i;
        }

        int predicateScriptNumber = (int)EditorGUILayout.IntPopup(label, lastPredicateScriptNumber == -1 ? 0 : lastPredicateScriptNumber, predicateScriptNames.ToArray(), predicateScriptOptions.ToArray());

        if (lastPredicateScriptNumber != predicateScriptNumber)
        {
            predicateScript = (MonoScript)AssetDatabase.LoadAssetAtPath(predicateScriptPaths[predicateScriptNumber], typeof(MonoScript));
            predicate = (CardEffectPredicate)Activator.CreateInstance(predicateScript.GetClass());
        } 
    }
#endif
}
