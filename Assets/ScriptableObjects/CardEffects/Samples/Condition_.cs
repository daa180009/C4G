using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition_S : CardEffectCondition
{
#if UNITY_EDITOR
    public override void InputGUI()
    {
    }
#endif

    public override bool CheckCondition(WorldInfo worldInfo, ResolutionInfo resolutionInfo)
    {
        return true;
    }

    public override string GetDescription(WorldInfo worldInfo)
    {
        return "";
    }
}
