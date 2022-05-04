using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Scriptable Object that holds the data for each separate stage, notably the tile types and dimensions of the tiles for the stage
/// Only serializable information can be stored here because its a ScriptableObject so beware
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StageData", order = 1), System.Serializable]
public class StageData : ScriptableObject, ISerializationCallbackReceiver
{
    Tile.TileType[,] unserializedValues;
    [SerializeField, HideInInspector] private List<Package<Tile.TileType>> serializable;

    /// <summary>
    /// The in-game name of the stage; filename is not stored with StageData object
    /// </summary>
    public string StageTitle;

    public int Width;
    public int Height;

    /// <summary>
    /// Public accessor
    /// </summary>
    public Tile.TileType[,] Values
    {
        get
        {
            return unserializedValues;
        }
    }

    /// <summary>
    /// The color associated with each tile type to be displayed in each button
    /// </summary>
    private Dictionary<Tile.TileType, Color> tileColor = new Dictionary<Tile.TileType, Color>()
    {
        { Tile.TileType.None, Color.white },
        { Tile.TileType.Floor, Color.white },
        { Tile.TileType.Wall, Color.black },
        { Tile.TileType.Raised, new Color(.75f, .75f, .75f) }, // a lighter grey than the barrier
        { Tile.TileType.Barrier, Color.grey },
        { Tile.TileType.Entrance, Color.green },
        { Tile.TileType.Exit, Color.red },
    };

    /// <summary>
    /// The current tile type that the user has selected to draw with
    /// </summary>
    private Tile.TileType paintingType = Tile.TileType.Floor;

#if UNITY_EDITOR
    public void OnInputGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 45f;
        StageTitle = EditorGUILayout.TextField("Title:", StageTitle, GUILayout.Width(140));
        int initialWidth = Width;
        int initialHeight = Height;
        Width = EditorGUILayout.IntField("Width: ", Width);
        Height = EditorGUILayout.IntField("Height: ", Height);
        
        // Limited the stage width and height to 0-100 
        Width = Mathf.Clamp(Width, 0, 100);
        Height = Mathf.Clamp(Height, 0, 100);

        if(unserializedValues == null)
        {
            unserializedValues = new Tile.TileType[Width, Height];
        }
        else if(Width != initialWidth || Height != initialHeight)
        {
            Tile.TileType[,] replacementValues = new Tile.TileType[Width, Height];
            for(int x = 0; x < unserializedValues.GetLength(0) && x < Width; x++)
            {
                for(int y = 0; y < unserializedValues.GetLength(1) && y < Height; y++)
                {
                    replacementValues[x, y] = unserializedValues[x, y];
                }
            }
            unserializedValues = replacementValues;
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        // User selection for which tile type to draw with
        GUI.backgroundColor = tileColor[paintingType];
        paintingType = (Tile.TileType)EditorGUILayout.EnumPopup(paintingType);

        // Create a matrix of buttons, each for one tile. Clicking them changes their tile type to the user's selected
        for (int y = 0; y < Height; y++)
        {
            // have to make a new horizontal display for each line of buttons
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < Width; x++)
            {
                // set button colour to current tile type
                GUI.backgroundColor = tileColor[unserializedValues[x, y]];
                if (GUILayout.Button("", GUILayout.MaxWidth(30), GUILayout.Height(Mathf.Min(30, Screen.width / Width))))
                {
                    //if they already have this tile type, switch it back to floor instead
                    if (unserializedValues[x, y] == paintingType)
                        unserializedValues[x, y] = Tile.TileType.Floor;
                    else
                        unserializedValues[x, y] = paintingType;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space(5);
    
    }
#endif

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
        serializable = new List<Package<Tile.TileType>>();
        for (int i = 0; i < unserializedValues.GetLength(0); i++)
        {
            for (int j = 0; j < unserializedValues.GetLength(1); j++)
            {
                serializable.Add(new Package<Tile.TileType>(i, j, unserializedValues[i, j]));
            }
        }
    }
    public void OnAfterDeserialize()
    {
        // Convert the serializable list into our unserializable array
        unserializedValues = new Tile.TileType[Width, Height];
        foreach (var package in serializable)
        {
            unserializedValues[package.Index0, package.Index1] = package.Element == Tile.TileType.None ? Tile.TileType.Floor : package.Element;
        }
    }
}

