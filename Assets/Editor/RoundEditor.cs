using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoundData), true)]
public class RoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoundData data = (RoundData)target;
        data.OnInputGUI();
        EditorUtility.SetDirty(data);
    }
}
