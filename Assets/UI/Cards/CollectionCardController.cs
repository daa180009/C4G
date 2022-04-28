using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CollectionCardController : MonoBehaviour
{
    public GameObject countPipsPrefab;

    public GameObject countPipsContainer;
    public VisualCardController VisualCard;

    /// <summary>
    /// The data for the card being counted
    /// </summary>
    public CardData Data
    {
        get { return VisualCard.Data; }
        set { VisualCard.Data = value; }
    }

    int count;
    /// <summary>
    /// The amount of the cards that have been added to the deck so far.
    /// </summary>
    public int Count
    {
        get { return count;  }
        set
        {
            count = value;
            updateCountPips();
        }
    }

    public int MaxCount
    {
        get { return 4; }
    }

    Action<CollectionCardController> cbCountChanged;
    /// <summary>
    /// Register a function to be called when the number of cards played changes
    /// </summary>
    public void RegisterCountChanged(Action<CollectionCardController> cb) { cbCountChanged -= cb; cbCountChanged += cb; }
    public void UnregisterCountChanged(Action<CollectionCardController> cb) { cbCountChanged -= cb; }

    public void Awake()
    {
        Count = 0;
    }

    public void Start()
    {
        VisualCard.RegisterClicked(IncrementCount);
        VisualCard.RegisterRightClicked(DecrementCount);
    }

    void updateCountPips()
    {
        foreach(Transform child in countPipsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < MaxCount; i++)
        {
            GameObject pipObject = Instantiate(countPipsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            CountPipController pipController = pipObject.GetComponent<CountPipController>();

            pipObject.transform.SetParent(countPipsContainer.transform);
            pipController.IsOn = i < count;
        }
    }

    void IncrementCount(VisualCardController card)
    {
        if (count < 4)
            count = count + 1;
        updateCountPips();

        if (cbCountChanged != null)
            cbCountChanged(this);
    }

    void DecrementCount(VisualCardController card)
    {
        if (count > 0)
            count = count - 1;
        updateCountPips();

        if (cbCountChanged != null)
            cbCountChanged(this);
    }
}
