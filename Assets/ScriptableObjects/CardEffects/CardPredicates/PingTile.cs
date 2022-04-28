using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class PingTile : CardEffectPredicate
{
    public override Card.TargetType TargetType { get { return Card.TargetType.Tiles; } }

    public int length = 60;

#if UNITY_EDITOR
    public override void InputGUI()
    {
        length = EditorGUILayout.IntField("Ping Time: ", length);
    }
#endif

    public override void PerformPredicate(TargetInfo targetInfo, WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        foreach (TileController tile in targetInfo.Tiles)
            tile.Ping(length);
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
