using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Tile
{
    [Flags, System.Serializable]
    public enum TileType { 
        None, //
        Floor = 1 << 0, // A
        Wall = 1 << 1, // B 
        Raised = 1 << 2, // C
        Barrier = 1 << 3, // D
        Entrance = 1 << 4,  // E
        Exit = 1 << 5 // F
    };

    [Flags]
    public enum TileDirection
    {
        None = 0,
        Left = 1 << 0,
        Up = 1 << 1,
        Right = 1 << 2,
        Down = 1 << 3,
    };
}
