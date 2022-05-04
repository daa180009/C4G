using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SacTower : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Towers; } }

#if UNITY_EDITOR
    public override void InputGUI()
    {
        
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (TowerController tower in targetInfo.Towers)
        {
            tower.DestroySelf();
        }
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "sacrifice that tower";
    }
}
