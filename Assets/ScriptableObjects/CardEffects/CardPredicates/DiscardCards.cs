using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DiscardCards : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }

    public int cardAmount = 1;

#if UNITY_EDITOR
    public override void InputGUI()
    {
        cardAmount = EditorGUILayout.IntField("# of cards: ", cardAmount);
        if(cardAmount < 0)
            cardAmount = 0;
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        worldInfo.cardGameController.DiscardCard(cardAmount);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "discard " + cardAmount + " card" + ((cardAmount != 1) ? "s" : "") + ".";
    }
}
