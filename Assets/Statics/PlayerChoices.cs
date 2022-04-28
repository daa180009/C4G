using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// static class for transferring data from game start screen to game level
// probably should use a better pattern later but this is good for now
public static class PlayerChoices 
{
    public static DeckListData DeckList { get; set; }
}
