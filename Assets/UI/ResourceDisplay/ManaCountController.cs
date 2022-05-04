using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCountController : MonoBehaviour
{
    public CostIcon costIcon;
    public Text countText;

    public Mana.ManaType manaType;

    public void Start()
    {
        costIcon.Type = manaType;  
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}
