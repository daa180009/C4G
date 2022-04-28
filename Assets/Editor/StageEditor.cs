using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageData), true)]
public class StageDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StageData data = (StageData)target;
        data.OnInputGUI();
        EditorUtility.SetDirty(data);
    }
}