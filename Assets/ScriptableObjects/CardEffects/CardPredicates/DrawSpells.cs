using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DrawSpells : CardEffectPredicate
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
        for (int i = 0; i < cardAmount; i++)
        {
            worldInfo.cardGameController.DrawCard(1);
            if (worldInfo.HandZone.GetCard(worldInfo.HandZone.Count - 1).Data.Type == Card.CardType.Tower)
            {
                worldInfo.cardGameController.DiscardRight(1);
            }
        }
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "draw " + cardAmount + " card" + ((cardAmount != 1) ? "s" : "") + ". Tower cards go to discard instead.";
    }
}
