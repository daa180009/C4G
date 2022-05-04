using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DirectTowerDamage : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Towers; } }

    public float damage = 1f;

    /// <summary>
    /// Whether or not to apply a status to hit towers
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
        damage = EditorGUILayout.FloatField("Damage: ", damage);
        applyStatus = EditorGUILayout.Toggle("Apply Status?", applyStatus);
        if(applyStatus)
        {
            status = (Card.Status)EditorGUILayout.EnumPopup("Status", status);
            duration = EditorGUILayout.FloatField("Duration", duration);
        }
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (TowerController tower in targetInfo.Towers)
        {
            tower.DirectDamage(damage);

            if (applyStatus)
                tower.AddStatus(status, duration);
        }    
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        string returnString = "";
        if(damage > 0)
        {
            returnString += "deal " + damage + " damage to that tower";
            if (applyStatus)
                returnString += " and ";
        }

        if (applyStatus)
            returnString += "applies <b>" + CardData.GetStatusName(status) + "</b> to that tower for " + duration + "s.";

        return returnString;
    }
}
