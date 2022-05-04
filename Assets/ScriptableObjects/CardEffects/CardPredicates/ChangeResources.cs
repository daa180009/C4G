using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ChangeResources : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }

    public int hpChange = 0;
    public int landPlaysChange = 0;

    public int noneManaChange = 0;

#if UNITY_EDITOR
    public override void InputGUI()
    {
        hpChange = EditorGUILayout.IntField("HP change: ", hpChange);
        landPlaysChange = EditorGUILayout.IntField("Mana tower players change: ", landPlaysChange);
        noneManaChange = EditorGUILayout.IntField("Generic mana change: ", noneManaChange);
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        worldInfo.playerResourceManager.LifeTotal += hpChange;
        worldInfo.playerResourceManager.LandPlaysTotal += landPlaysChange;
        worldInfo.playerResourceManager.AddMana(Mana.ManaType.None, noneManaChange);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        string returnString = "";

        if (hpChange > 0)
        {
            returnString += "Gain " + hpChange + " life.";
        }
        else if (hpChange < 0)
        {
            returnString += "Lose " + -hpChange + " life.";
        }

        if (landPlaysChange > 0)
        {
            returnString += "You may play " + landPlaysChange + " addition mana tower" + (landPlaysChange == 1 ? "" : "s") + " this turn.";
        }
        else if (landPlaysChange < 0)
        {
            returnString += "You may play " + -landPlaysChange + " fewer mana tower" + (landPlaysChange == 1 ? "" : "s") + " this turn.";
        }

        if (noneManaChange > 0)
        {
            returnString += "Add " + noneManaChange + " generic mana.";
        }
        else if (noneManaChange < 0)
        {
            returnString += "Lose " + -noneManaChange + " <b>Neutral</b> mana.";
        }

        return returnString;
    }
}
