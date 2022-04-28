using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCountController : MonoBehaviour
{
    public Image iconImage;
    public Image iconImageBack;
    public Text countText;

    public Mana.ManaType manaType;

    public void Start()
    {
        iconImage.sprite = CardData.GetSpriteOfManaType(manaType);
        iconImage.color = CardData.GetColorOfManaType(manaType);
        iconImageBack.color = CardData.GetColorOfManaType(manaType).AdjustedBrightness(.7f);
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}
