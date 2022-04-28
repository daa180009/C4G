using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostIcon : MonoBehaviour
{
    public Image iconImage;

    public Mana.ManaType Type
    {
        set
        {
            iconImage.sprite = CardData.GetSpriteOfManaType(value);
            iconImage.color = CardData.GetColorOfManaType(value);
            GetComponent<Image>().color = CardData.GetColorOfManaType(value).AdjustedBrightness(.8f); 
        }
    }
}