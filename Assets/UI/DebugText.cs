using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text myText;
    public PlayerResourceManager playerResourceManager;
    public CardZone DiscardZone;
    public CardZone DeckZone;
    public CardZone HandZone;

    public int typeOfDebugText = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (typeOfDebugText)
        {
            case 0:
                myText.text = playerResourceManager.debugString();
                break;
            case 1:
                myText.text = "Hand Size: " + HandZone.Count + "\nDeck Size: " + DeckZone.Count + "\nDiscard Size: " + DiscardZone.Count;
                break;
            default:
                return;
        }
    }
}
