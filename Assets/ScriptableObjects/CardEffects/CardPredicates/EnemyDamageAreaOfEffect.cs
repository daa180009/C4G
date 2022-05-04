using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class EnemyDamageAreaOfEffect : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public float damage = 3f;

    /// <summary>
    /// Whether or not to apply a status to hit enemies
    /// </summary>
    public bool applyStatus = false;
    /// <summary>
    /// Which status to apply if so
    /// </summary>
    public Card.Status status;
    /// <summary>
    /// How long said status should last upon application
    /// </summary>
    public float duration;

#if UNITY_EDITOR
    public override void InputGUI()
    {
        damage = EditorGUILayout.FloatField("Damage", damage);
        applyStatus = EditorGUILayout.Toggle("Apply Status?", applyStatus);
        if(applyStatus)
        {
            status = (Card.Status)EditorGUILayout.EnumPopup("Status", status);
            duration = EditorGUILayout.FloatField("Duration", duration);
        }

        AffectedArea = EditorGUILayout.ObjectField("AOE:", AffectedArea, typeof(AreaOfEffect), true) as AreaOfEffect;
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        targetInfo.AOETargetting(AffectedArea, worldInfo, targetInfo.Direction);

        foreach(EnemyController enemy in targetInfo.AOEEnemies[1])
        {
            enemy.DirectDamage(damage);

            if (applyStatus)
                enemy.AddStatus(status, duration);
        }
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        string returnString = "deal " + damage + " damage to each enemy adjacent to that tile "; 
        if(applyStatus)
            returnString += "and applies <b>" + CardData.GetStatusName(status) + "</b> to those enemies for " + duration +  "s.";
        return returnString;
    }
}
