using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceManager : ScriptableObject
{
    Dictionary<Mana.ManaType, int> manaTotal;
    public Dictionary<Mana.ManaType, int> ManaTotal
    {
        get
        {
            return manaTotal;
        }
    }

    public bool isDebug = false;

    int lifeTotal;
    public int LifeTotal { get { return lifeTotal; } set { lifeTotal = value; } }

    int wisdomTotal;
    public int WisdomTotal { get { return wisdomTotal; } set { wisdomTotal = value; } }

    int creativityTotal;
    public int CreativityTotal { get { return creativityTotal; } set { creativityTotal = value; } }

    int landPlaysTotal;
    public int LandPlaysTotal { get { return landPlaysTotal; } set { landPlaysTotal = value; } }

    public void OnEnable()
    {
        Reset();
    }

    public string debugString()
    {
        string returnString = "";
        foreach (KeyValuePair<Mana.ManaType, int> kvp in manaTotal)
        {
            returnString += kvp.Key.ToString() + ": " + kvp.Value + "\n";
        }
        returnString += "Life: " + lifeTotal + "\n";
        returnString += "Wisdom: " + wisdomTotal + "\n";
        returnString += "Creativity: " + creativityTotal + "\n";

        return returnString;
    }

    public bool CanAfford(CardData data)
    {
        if (isDebug)
            return true;

        if (data.TowerSubtypes.HasFlag(Card.TowerSubtype.Mana) && landPlaysTotal <= 0)
            return false;

        return CanAfford(data.ManaCostDictionary);
    }

    public bool CanAfford(Dictionary<Mana.ManaType, int> cost)
    {
        if (isDebug)
            return true;

        int totalUnused = 0;
        foreach (KeyValuePair<Mana.ManaType, int> kvp in cost)
        {
            if(kvp.Key != Mana.ManaType.None)
            {
                if(manaTotal[kvp.Key] < kvp.Value)
                    return false;
                else
                    totalUnused += manaTotal[kvp.Key] - kvp.Value;
            }
            else
            {
                totalUnused += manaTotal[kvp.Key];
            }
        }

        return totalUnused >= cost[Mana.ManaType.None];
    }

    public void PayCost(CardData data)
    {
        if (data.TowerSubtypes.HasFlag(Card.TowerSubtype.Mana))
            landPlaysTotal -= 1;

        PayCost(data.ManaCostDictionary);
    }

    public void PayCost(Dictionary<Mana.ManaType, int> cost)
    {
        // pay non-generic costs
        foreach (KeyValuePair<Mana.ManaType, int> kvp in cost)
            if (kvp.Key != Mana.ManaType.None)
                manaTotal[kvp.Key] -= kvp.Value;

        // pay generic costs
        int remainingGeneric = cost[Mana.ManaType.None];
        foreach (KeyValuePair<Mana.ManaType, int> kvp in cost)
        {
            if(manaTotal[kvp.Key] >= remainingGeneric)
            {
                manaTotal[kvp.Key] -= remainingGeneric;
                remainingGeneric = 0;
            } 
            else
            {
                remainingGeneric -= manaTotal[kvp.Key];
                manaTotal[kvp.Key] = 0;
            }
        }
    }

    public void Reset()
    {
        manaTotal = new Dictionary<Mana.ManaType, int>()
        {
            { Mana.ManaType.None, 0 },
            { Mana.ManaType.Clubs, 0 },
            { Mana.ManaType.Spades, 0 },
            { Mana.ManaType.Hearts, 0},
            { Mana.ManaType.Diamonds, 0 }
        };
        lifeTotal = 20;
        wisdomTotal = 0;
        creativityTotal = 0;
        landPlaysTotal = 1;
    }

    public void AddMana(Mana.ManaType type, int amount)
    {
        manaTotal[type] += amount;
        if (manaTotal[type] <= 0)
            manaTotal[type] = 0;
    }
}