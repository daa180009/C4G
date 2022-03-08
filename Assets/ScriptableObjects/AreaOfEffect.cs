using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AreaOfEffect : ISerializationCallbackReceiver
{
    int[,] unserializedValues = new int[25, 25];
    [SerializeField, HideInInspector] private List<Package<int>> serializable;

    public int Width;
    public int Height;

    public int[,] Values
    {
        get
        {
            return unserializedValues;
        }
    }

    string[] names = new string[] { "0", "1" };
    /// <summary>
    /// The names for each option. Setting 'max' will reset these names so do that first.
    /// </summary>
    public string[] Names
    {
        set
        {
            names = value;
            for (int i = 0; i < names.Length; i++)
                names[i] = i + " - " + names[i];
        }
    }

    int[] options = new int[] { 0, 1 };
    int max = 1;
    /// <summary>
    /// The maximum value allowed for a relative tile. Set to automatically set names as well.
    /// </summary>
    public int Max
    {
        set
        {
            if(max >= 1)
            {
                max = value;

                options = new int[value + 1];
                names = new string[value + 1];

                for (int i = 0; i <= value; i++)
                {
                    options[i] = i;
                    names[i] = i.ToString();
                }
            }
        }

        get
        {
            return max;
        }
    }

    public Dictionary<int, Color> ButtonColor = new Dictionary<int, Color>()
    {
        { -1, Color.blue },
        { 0, Color.grey },
        { 1, Color.green }
    };

    int currentBrush = 0;

    public void OnInputGUI()
    {
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        int originalIndentLevel = EditorGUI.indentLevel;

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 45f;
        EditorGUI.indentLevel = 0;
        GUILayout.FlexibleSpace();

        Width = (int)EditorGUILayout.IntField("Width: ", Width, GUILayout.Width(70));
        EditorGUILayout.Space(); 
        Height = (int)EditorGUILayout.IntField("Height: ", Height, GUILayout.Width(70));
        EditorGUILayout.Space();

        if (ButtonColor.ContainsKey(currentBrush))
            GUI.backgroundColor = ButtonColor[currentBrush];
        else
            GUI.backgroundColor = Color.grey;
        currentBrush = (int)EditorGUILayout.IntPopup("Brush: ", currentBrush, names, options, GUILayout.Width(160));

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (Width % 2 == 0)
            Width += 1;
        if (Height % 2 == 0)
            Height += 1;

        if (!options.Contains(currentBrush))
            currentBrush = 0;

        GUILayoutOption buttonWidth = GUILayout.MaxWidth(30);
        GUILayoutOption buttonHeight = GUILayout.Height(Mathf.Min(30, Screen.width / Width));
        for (int y = 0; y < Height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int x = 0; x < Width; x++)
            {
                if (ButtonColor.ContainsKey(unserializedValues[x, y]))
                    GUI.backgroundColor = ButtonColor[unserializedValues[x, y]];
                else
                    GUI.backgroundColor = Color.grey;

                if (y == Height / 2 && x == Width / 2)
                {
                    unserializedValues[x, y] = -1;
                    GUILayout.Button(unserializedValues[x, y].ToString(), buttonWidth, buttonHeight);
                }
                else
                {
                    if (GUILayout.Button(unserializedValues[x, y].ToString(), buttonWidth, buttonHeight))
                    {
                        if (unserializedValues[x, y] != currentBrush)
                            unserializedValues[x, y] = currentBrush;
                        else
                            unserializedValues[x, y] = 0;
                    }

                    if (!options.Contains(unserializedValues[x,y]))
                        unserializedValues[x,y] = 0;
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUI.indentLevel = originalIndentLevel;
    }

    [System.Serializable]
    struct Package<TElement>
    {
        public int Index0;
        public int Index1;
        public TElement Element;
        public Package(int idx0, int idx1, TElement element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Element = element;
        }
    }

    public void OnBeforeSerialize()
    {
        // Convert our unserializable array into a serializable list
        serializable = new List<Package<int>>();
        for (int i = 0; i < unserializedValues.GetLength(0); i++)
        {
            for (int j = 0; j < unserializedValues.GetLength(1); j++)
            {
                serializable.Add(new Package<int>(i, j, unserializedValues[i, j]));
            }
        }
    }
    public void OnAfterDeserialize()
    {
        // Convert the serializable list into our unserializable array
        unserializedValues = new int[25, 25];
        foreach (var package in serializable)
        {
            unserializedValues[package.Index0, package.Index1] = package.Element;
        }
    }
}
