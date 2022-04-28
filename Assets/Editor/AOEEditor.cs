using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AreaOfEffect), true)]
public class AreaOfEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AreaOfEffect area = (AreaOfEffect)target;
        area.OnInputGUI();
        EditorUtility.SetDirty(area);
    }
}