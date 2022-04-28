using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Revive : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.None; } }


#if UNITY_EDITOR
    public override void InputGUI()
    {
    
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        Debug.Log("bepis");
        worldInfo.cardGameController.PlayYardTower();
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "play the top tower card from the graveyard.";
    }
}
