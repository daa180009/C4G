using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class YardReliantDraw : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }

    public int perYardCard = 1;
    

#if UNITY_EDITOR
    public override void InputGUI()
    {
        perYardCard = EditorGUILayout.IntField("# of cards in 'yard: ", perYardCard);
        if(perYardCard < 0)
            perYardCard = 0;
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        int yardCount = worldInfo.DiscardZone.Count;
        int cardAmount = yardCount / perYardCard;
        worldInfo.cardGameController.DrawCard(cardAmount);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "draw a card for every " + perYardCard + " card" + ((perYardCard != 1) ? "s" : "") + " in discard.";
    }
}
