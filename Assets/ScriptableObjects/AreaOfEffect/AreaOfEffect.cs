using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AreaOfEffect : ScriptableObject, ISerializationCallbackReceiver
{
    int[,] unserializedValues;
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

    string[] names = new string[] { "Category 0", "Category 1" };
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
                    names[i] = "Category " + i.ToString();
                }
            }
        }

        get
        {
            return max;
        }
    }

    Dictionary<int, Color> ButtonColor = new Dictionary<int, Color>()
    {
        { 0, Color.grey },
        { 1, Color.green }
    };

    int currentBrush = 0;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/ScriptableObjects/Area Of Effect 3x3", false, 10)]
    public static void CreateAreaOfEffect3x3()
    {
        AreaOfEffect area = ScriptableObject.CreateInstance<AreaOfEffect>();
        area.Width = 3;
        area.Height = 3;
        area.unserializedValues = new int[3,3];

        AssetDatabase.CreateAsset(area, "Assets/ScriptableObjects/AreaOfEffect/3x3_.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = area;
    }

    [MenuItem("Assets/Create/ScriptableObjects/Area Of Effect 5x5", false, 10)]
    public static void CreateAreaOfEffect5x5()
    {
        AreaOfEffect area = ScriptableObject.CreateInstance<AreaOfEffect>();
        area.Width = 5;
        area.Height = 5;
        area.unserializedValues = new int[5, 5];

        AssetDatabase.CreateAsset(area, "Assets/ScriptableObjects/AreaOfEffect/5x5_.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = area;
    }

    public void OnInputGUI()
    {
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        int originalIndentLevel = EditorGUI.indentLevel;

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 45f;
        EditorGUI.indentLevel = 0;
        GUILayout.FlexibleSpace();

        /*Width = (int)EditorGUILayout.IntField("Width: ", Width, GUILayout.Width(70));
        EditorGUILayout.Space(); 
        Height = (int)EditorGUILayout.IntField("Height: ", Height, GUILayout.Width(70));
        EditorGUILayout.Space();

        Width = Mathf.Clamp(Width, 1, 25);
        Height = Mathf.Clamp(Height, 1, 25);
        if (Width % 2 == 0)
            Width += 1;
        if (Height % 2 == 0)
            Height += 1;*/

        if (ButtonColor.ContainsKey(currentBrush))
            GUI.backgroundColor = ButtonColor[currentBrush];
        else
            GUI.backgroundColor = Color.grey;
        currentBrush = (int)EditorGUILayout.IntPopup("Brush: ", currentBrush, names, options, GUILayout.Width(160));
        if (!options.Contains(currentBrush))
            currentBrush = 0;

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

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
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUI.indentLevel = originalIndentLevel;
    }
#endif

    public int[,] GetRotated(Tile.TileDirection direction)
    {
        switch (direction)
        {
            case Tile.TileDirection.Left: return unserializedValues;
            case Tile.TileDirection.Up: return unserializedValues.RotatedRight();
            case Tile.TileDirection.Right: return unserializedValues.RotatedRight().RotatedRight();
            case Tile.TileDirection.Down: return unserializedValues.RotatedLeft();
        }
        return unserializedValues;
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
        unserializedValues = new int[Height, Width];
        foreach (var package in serializable)
        {
            unserializedValues[package.Index0, package.Index1] = package.Element;
        }
    }
}
