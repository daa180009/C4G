using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Towers_NoQuality : CardEffectQuality
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Towers; } }

#if UNITY_EDITOR
    public override void InputGUI()
    {
    }
#endif

    public override bool CheckQuality(TileController tileController, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return true;
    }

    public override string GetDescription(WorldInfo worldInfo, bool isPlural)
    {
        return isPlural ? "towers" : "tower";
    }
}
