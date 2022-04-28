using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardData), true)]
public class CardDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardData data = (CardData)target;
        data.OnInputGUI();
        EditorUtility.SetDirty(data);
    }
}