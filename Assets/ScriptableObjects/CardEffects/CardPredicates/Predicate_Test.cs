using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Predicate_Test : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public override void InputGUI()
    {
    }

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (TileController tile in targetInfo.Tiles)
            tile.Ping(60);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
