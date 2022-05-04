using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerPositionData
{
    public CardData cardData;
    public int xPosition;
    public int yPosition;
    public Tile.TileDirection directionFacing;

    public TowerPositionData(TowerController tower, int x, int y)
    {
        xPosition = x;
        yPosition = y;

        cardData = tower.CardParent;
        directionFacing = tower.FacingDirection;
    }
}
