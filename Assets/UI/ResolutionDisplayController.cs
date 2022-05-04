using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResolutionDisplayController : MonoBehaviour
{
    public CardResolutionController cardResolutionController;

    public Text TargetCountText;
    public Button SubmitButton;

    public CardZone ResolutionZone;

    void Awake()
    {
        ResolutionZone.RegisterCardsAdded(onCardsAdded);
        ResolutionZone.RegisterCardsRemoved(onCardsRemoved);
        cardResolutionController.RegisterTargetCountChanged(onTargetCountChanged);
    }

    void onCardsAdded(List<CardController> addedCards)
    {
        foreach(CardController card in addedCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(this.transform);
            card.transform.SetAsFirstSibling();

            card.horizontalEdge = RectTransform.Edge.Right;
            card.TargetX = 15;
            card.TargetY = 35;
            card.TargetRotation = 0;
        }
    }

    void onCardsRemoved(List<CardController> removedCards)
    {
        /*
        foreach (CardController card in removedCards)
        {
            card.gameObject.SetActive(false);
        }
        */
    }

    void onTargetCountChanged(int currentTargets, int targetsMax, bool allowSubmit)
    {
        if(targetsMax == 0)
            TargetCountText.text = "Submit";
        else
            TargetCountText.text = "Submit (" + currentTargets + " / " + targetsMax + ")";

        SubmitButton.gameObject.SetActive(allowSubmit);
    }
}
