using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object to use when resolving card effects so that the card scripts have access to all the info they need
/// to check conditions and modify the world.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WorldInfo", order = 1)]
public class WorldInfo : ScriptableObject
{
    public CardZone DiscardZone;
    public CardZone HandZone;
    public CardZone DeckZone;
    public CardZone ResolutionZone;

    public CardGameController cardGameController { set; get; }
    public WorldController worldController { set; get; }
}
