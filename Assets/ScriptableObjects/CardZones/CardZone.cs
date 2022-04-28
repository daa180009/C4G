using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardZone", order = 1), System.Serializable]
public class CardZone : ScriptableObject
{
    public GameObject cardPrefab;

    /// <summary>
    /// Whether or not order matters in this zone. Typically only true for the deck
    /// </summary>
    public bool isOrdered;

    /// <summary>
    /// The number of cards in the zone
    /// </summary>
    public int Count { get { return cards.Count; } }

    /// <summary>
    /// The list of cards in the zone currently
    /// </summary>
    [System.NonSerialized]
    List<CardController> cards = new List<CardController>();

    Action<List<CardController>> cbCardsAdded;
    /// <summary>
    /// Register a function to be called whenever one or more new cards enters this zone
    /// </summary>
    public void RegisterCardsAdded(Action<List<CardController>> cb) { cbCardsAdded += cb; }

    Action<List<CardController>> cbCardsRemoved;
    /// <summary>
    /// Register a function to be called whenever one or more new cards are removed from this zone
    /// </summary>
    public void RegisterCardsRemoved(Action<List<CardController>> cb) { cbCardsRemoved += cb; }

    /// <summary>
    /// Create new card models for the card data passed in then add it to this zone
    /// </summary>
    public List<CardController> ConjureList(List<CardData> cardDataList)
    {
        List<CardController> addedCards = new List<CardController>();
        foreach (CardData cardData in cardDataList)
        {
            CardController newCard = Instantiate(cardPrefab, new Vector3(0,0,0), Quaternion.identity).GetComponent<CardController>();
            newCard.Data = cardData;

            cards.Add(newCard);
            addedCards.Add(newCard);
        }

        if(cbCardsAdded != null)
            cbCardsAdded(addedCards);
        return addedCards;
    }

    /// <summary>
    /// Randomize the order of the cards in the zone
    /// </summary>
    public void Shuffle()
    {
        if(isOrdered)
            cards.Shuffle();
    }

    /// <summary>
    /// For each card in the zone, call a function.
    /// Function inputs are [Card, card index, number of cards]
    /// </summary>
    public void ForEach(Action<CardController, int, int> action)
    {
        for (int i = 0; i < cards.Count; i++)
            action(cards[i], i, cards.Count);
    }

    /// <summary>
    /// Move all the cards from one zone into this one
    /// </summary>
    public void AddAll(CardZone otherZone)
    {
        while(otherZone.Count > 0)
        {
            CardController removedCard = otherZone.Pop();
            Add(removedCard);
        }
    }

    // might want better methods for moving between zones
    public void Add(CardController card)
    {
        if (cards.Contains(card))
            return;

        cards.Add(card);

        if (cbCardsAdded != null)
            cbCardsAdded(card.IndividualList());
    }

    public bool Remove(CardController card)
    {
        if(cards.Contains(card))
        {
            cards.Remove(card);

            if (cbCardsRemoved != null)
                cbCardsRemoved(card.IndividualList());

            return true;
        }
        return false;
    }

    public CardController Pop()
    {
        CardController removedCard = cards[0];
        cards.RemoveAt(0);

        if (cbCardsRemoved != null)
            cbCardsRemoved(removedCard.IndividualList());
        return removedCard;
    }

    public CardController PopRight()
    {
        int rightmost = Count - 1;
        if (rightmost < 0)
        {
            rightmost = 0;
        }
        CardController removedCard = cards[rightmost];
        cards.RemoveAt(rightmost);

        if (cbCardsRemoved != null)
            cbCardsRemoved(removedCard.IndividualList());
        return removedCard;
    }

    public CardController GetCard(int value)
    {
        CardController searchedCard = cards[value];
        return searchedCard;
    }

    public CardController PopSpecific(int value)
    {
        CardController removedCard = cards[value];
        cards.RemoveAt(value);

        if (cbCardsRemoved != null)
            cbCardsRemoved(removedCard.IndividualList());
        return removedCard;
    }
}
