using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CardEffect
{
    public static int InfiniteTarget = 4;
    public int minTargets = 0;
    public int maxTargets = 0;

    public string overrideDescription = "";

#if UNITY_EDITOR
    public MonoScript conditionScript;
    public MonoScript qualityScript;
    public MonoScript predicateScript;

    public void OnInputGUI()
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        string targetString;
        if (maxTargets == InfiniteTarget)
        {
            if (minTargets == InfiniteTarget)
                targetString = "all";
            else
                targetString = minTargets + "+";
        }
        else if (maxTargets == 0)
        {
            targetString = "none";
        }
        else
        {
            targetString = minTargets + "-" + maxTargets;
        }
        EditorGUILayout.LabelField("Targets (" + targetString + ")");
        /// theres no version of MinMaxSlider for ints which made me want to tear my hair out....
        float minTargetsFloat = minTargets; float maxTargetsFloat = maxTargets;
        EditorGUILayout.MinMaxSlider(ref minTargetsFloat, ref maxTargetsFloat, 0, InfiniteTarget);
        minTargets = (int)minTargetsFloat; maxTargets = (int)maxTargetsFloat;
        EditorGUILayout.EndHorizontal();

        CardEffectCondition.LoadCondition(ref conditionScript, ref condition);
        if (condition != null)
            condition.OnInputGUI();
        EditorGUILayout.Space(4);

        CardEffectPredicate.LoadPredicate(ref predicateScript, ref predicate);
        if (predicate != null)
            predicate.OnInputGUI();
        EditorGUILayout.Space(4);

        if (predicate != null && predicate.TargetType != Card.TargetType.None)
        {
            CardEffectQuality.LoadQuality(ref qualityScript, ref targetQuality, predicate.TargetType);
            if (targetQuality != null)
                targetQuality.OnInputGUI();
        }
        EditorGUILayout.Space(4);

        if (predicate != null && targetQuality != null && predicate.TargetType != targetQuality.TargetType)
        {
            qualityScript = null;
            targetQuality = null;
        }

        overrideDescription = EditorGUILayout.TextField("Override Description:", overrideDescription);

        EditorGUI.indentLevel--;
    }

    /// <summary>
    ///  GUI configuration for the card's condition information
    /// </summary>
    void conditionInfo()
    {
        MonoScript lastConditionScript = conditionScript;
        conditionScript = (MonoScript)EditorGUILayout.ObjectField("Condition", conditionScript, typeof(MonoScript), false);
        if (conditionScript == null)
        {
            string[] result = AssetDatabase.FindAssets("Condition_NoCondition");

            if (result.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                conditionScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            }
        }

        if (conditionScript != lastConditionScript)
            condition = (CardEffectCondition)Activator.CreateInstance(conditionScript.GetClass());

        if (condition != null)
            condition.OnInputGUI();
    }

    /// <summary>
    ///  GUI configuration for the card's targetting quality
    /// </summary>
    void targetQualityInfo()
    {

        MonoScript lastQualityScript = qualityScript;
        qualityScript = (MonoScript)EditorGUILayout.ObjectField("Quality", qualityScript, typeof(MonoScript), false);
        if (qualityScript == null)
        {
            string[] result = AssetDatabase.FindAssets("Quality_Tiles_NoQuality");

            if (result.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                qualityScript = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
            }
        }

        if (qualityScript != lastQualityScript)
            targetQuality = (CardEffectQuality)Activator.CreateInstance(qualityScript.GetClass());

        if (targetQuality != null)
            targetQuality.OnInputGUI();
    }
#endif

    [SerializeReference] public CardEffectCondition condition;
    [SerializeReference] public CardEffectQuality targetQuality;
    [SerializeReference] public CardEffectPredicate predicate;

    /// <summary>
    /// Textual description of the effect
    /// </summary>
    public string GetDescription(WorldInfo worldInfo) {
        // no targets so just ignore targetting, or no predicate to get type from
        if (condition == null || predicate == null)
        {
            return "";
        }

        if (overrideDescription != "")
            return overrideDescription;

        string returnString = condition.GetDescription(worldInfo);

        if(maxTargets > 0)
        {
            /// if maxTargets is 0, card does not target at all, so dont even display qualities
            
            // need plural for qualities, ie "enemy" vs "enemies"
            bool isPlural = true;
            if (maxTargets == InfiniteTarget)
            {
                // no limited on how many elements the user can target
                if (minTargets == InfiniteTarget)
                {
                    // if both are maxed out, we simply affect every single applicable element
                    returnString += "For each ";
                    isPlural = false;
                }
                else if (minTargets > 0)
                {
                    // user has to select at least one target but no other restrictions
                    returnString += "For " + minTargets.ToWord() + " or more target ";
                }
                else
                {
                    // essentially no restrictions on number of targets
                    returnString += "For any number of target ";
                }
            }
            else
            {
                // upper bound on number of targets user can choose
                if (minTargets > 0)
                {
                    // lower bound as well
                    if(minTargets == 1 && maxTargets == 1)
                    {
                        // user must select EXACTLY one
                        returnString += "For one target ";
                        isPlural = false;
                    }
                    else
                    {
                        // 
                        returnString += "For each of ";
                        for (int i = minTargets; i <= maxTargets; i++)
                        {
                            returnString += i.ToWord();
                            if (i != maxTargets && (maxTargets - minTargets == 1))
                                returnString += " or ";
                            else if (i != maxTargets)
                                returnString += ", or ";
                        }
                        returnString += " target ";
                    }
                }
                else
                {
                    // user can choose to not select any targets, or up to max
                    returnString += "For each of up to " + maxTargets.ToWord() + " target ";
                    if (maxTargets == 1)
                        isPlural = false;
                }
            }

            returnString += targetQuality.GetDescription(worldInfo, isPlural);
        }

        if(predicate != null)
            if (returnString == "")
                returnString += predicate.GetDescription(worldInfo).FirstCharToUpper();
            else
                returnString += ", " + predicate.GetDescription(worldInfo);

        return returnString;
    }
}
