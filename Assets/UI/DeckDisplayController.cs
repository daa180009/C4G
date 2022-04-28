using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplayController : MonoBehaviour
{
    public GameObject cardHolder;
    public Text SizeDisplay;

    /// <summary>
    /// The card zone representing the player's deck
    /// </summary>
    public CardZone DeckZone;

    void Awake()
    {
        DeckZone.RegisterCardsAdded(onCardsAdded);
        DeckZone.RegisterCardsRemoved(onCardsRemoved);
    }

    void Start()
    {
        CardVisualUpdate();
    }

    void CardVisualUpdate()
    {
        SizeDisplay.text = DeckZone.Count.ToString();
        DeckZone.ForEach(IndividualCardUpdate);
    }

    void IndividualCardUpdate(CardController card, int index, int count)
    {
        card.TargetScaleX = 0.01f;
        card.TargetScaleY = 0.01f;
        card.TargetX = -card.Width / 2f;
        card.TargetY = 10;
        card.horizontalEdge = RectTransform.Edge.Left;
    }

    void onCardsAdded(List<CardController> addedCards)
    {
        foreach (CardController card in addedCards)
        {
            card.transform.SetParent(cardHolder.transform);
        }

        CardVisualUpdate();
    }

    void onCardsRemoved(List<CardController> removedCards)
    {
        CardVisualUpdate();
    }
}
