using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HandDisplayController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardGameController cardGameController;
    public CardResolutionController cardResolutionController;
    public PlayerResourceManager playerResourceManager;
    public EnemySpawnController enemySpawnController;

    public float HorizontalCardDisplacement;
    public float VerticalCardDisplacement;
    public float RotationCardDisplacement;
    public float ResolvingVerticalDisplacement;
    public float HoveredCardHorizontalDisplacement;
    public float NonHoveredCardHorizontalDisplacement;
    public float HoveredCardVerticalDisplacement;
    public float HoveredCardHorizontalScale = 1f;
    public float HoveredCardVerticalScale = 1f;

    public Color UnplayableCardColor;
    public Color PlayableCardColor;

    public GameObject cardHolder;

    /// <summary>
    /// The card zone representing the player's hand
    /// </summary>
    public CardZone HandZone;

    /// <summary>
    /// The last card in hand to be hovered over by the user. Null if they haven't hovered
    /// </summary>
    CardController lastHovered = null;

    Action<bool> cbHoveredChanged;
    public void RegisterHoveredChanged(Action<bool> cb) { cbHoveredChanged += cb; }

    void Awake()
    {
        cardHolder = transform.GetChild(0).gameObject;
        HandZone.RegisterCardsAdded(onCardsAdded);
        HandZone.RegisterCardsRemoved(onCardsRemoved);
        cardResolutionController.RegisterResolutionFinished(onResolutionFinished);
    }

    int debugTimer = 0;
    void FixedUpdate()
    {
        debugTimer += 1;
        if(debugTimer > 10)
        {
            debugTimer = 0;
            CardVisualUpdate();
        }
    }

    /// <summary>
    /// Needs to be called whenever something about the visual cards changes so it can update their position
    /// </summary>
    void CardVisualUpdate()
    {
        bool pastHovered = false;

        // this is a wee mess
        List<float> yPositions = new List<float>() { 0 };
        List<float> yPositionsBase = new List<float>() { 0 };
        for (int i = 1; i < 4; i++)
        {
            yPositions.Add((float)Math.Sin(RotationCardDisplacement * Math.PI / 180f * i) / 2f + yPositionsBase[i - 1]);
            yPositionsBase.Add((float)Math.Sin(RotationCardDisplacement * Math.PI / 180f * i) + yPositionsBase[i - 1]);
        };

        HandZone.ForEach((CardController card, int index, int count) => IndividualCardUpdate(card, index, count, yPositions, ref pastHovered));
    }

    void IndividualCardUpdate(CardController card, int index, int count, List<float> yPositions, ref bool pastHovered)
    {
        RectTransform cardTransform = card.GetComponent<RectTransform>();

        card.horizontalEdge = RectTransform.Edge.Left;

        // set the scales back to normal to start off
        card.TargetScaleX = 1;
        card.TargetScaleY = 1;

        // set target x position
        card.TargetX = (card.Width - HorizontalCardDisplacement) * ((float)index - (float)count / 2f - .5f) + cardHolder.GetComponent<RectTransform>().rect.width / 2f;

        // set target y position, with cards in the middle being higher
        card.TargetY = VerticalCardDisplacement * cardTransform.rect.width * yPositions[(int)Math.Abs((float)index - ((float)count - 1f) / 2f)];
        if (cardResolutionController.IsBusy)
            card.TargetY += ResolvingVerticalDisplacement;

        // set target rotation angle
        card.TargetRotation = RotationCardDisplacement * -((float)index - (float)(count - 1) / 2f);

        // if a card is hovered we have a move others over
        if (lastHovered != null)
        {
            if (card == lastHovered)
            {
                pastHovered = true;
                // make it bigger
                card.TargetScaleX = HoveredCardHorizontalScale;
                card.TargetScaleY = HoveredCardVerticalScale;
                // shift since itll be bigger
                if (index < count / 2)
                    card.TargetX += HoveredCardHorizontalDisplacement * Math.Sign(card.TargetX);
                else if (index > count / 2 || (index == count / 2 && count % 2 == 0))
                    card.TargetX -= HoveredCardHorizontalDisplacement * Math.Sign(card.TargetX);
                card.TargetY = HoveredCardVerticalDisplacement;
                // unrotate it for clarity
                card.TargetRotation *= .25f;
            }
            else if (pastHovered)
            {
                // already updated the hovered card so shift right
                card.TargetX += NonHoveredCardHorizontalDisplacement;
            }
            else
            {
                // havent updated the hovered card yet so shift left
                card.TargetX -= NonHoveredCardHorizontalDisplacement;
            }
        }
        else
        {
            card.transform.SetSiblingIndex(index);
        }

        if (CanPlayCard(card))
        {
            card.TargetGlowAlpha = 1f;
            card.TargetBorderColor = PlayableCardColor;
        }
        else
        {
            card.TargetGlowAlpha = 0f;
            card.TargetBorderColor = UnplayableCardColor;
        }
    }

    bool CanPlayCard(CardController card)
    {
        return playerResourceManager.CanAfford(card.Data) && !enemySpawnController.spawnedAllEnemies;
    }

    void onCardHover(CardController cardController)
    {
        lastHovered = cardController;
        cardController.transform.SetAsLastSibling();
        CardVisualUpdate();
    }

    void onCardUnhover(CardController cardController)
    {
        lastHovered = null;
        CardVisualUpdate();
    }

    void onCardPlayed(CardController cardController)
    {
        if (cardGameController.AttemptPlay(cardController))
            CardVisualUpdate();
    }

    void onCardsAdded(List<CardController> addedCards)
    {
        foreach(CardController card in addedCards)
        {
            card.gameObject.SetActive(true);
            card.transform.SetParent(cardHolder.transform);

            card.RegisterHovered(onCardHover);
            card.RegisterUnhovered(onCardUnhover);
            card.RegisterPlayed(onCardPlayed);
        }

        CardVisualUpdate();
    }

    void onCardsRemoved(List<CardController> removedCards)
    {
        foreach (CardController card in removedCards)
        {
            card.UnregisterHovered(onCardHover);
            card.UnregisterUnhovered(onCardUnhover);
            card.UnregisterPlayed(onCardPlayed);
        }
    }

    void onResolutionFinished(CardController resolvedCard)
    {
        CardVisualUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cbHoveredChanged(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cbHoveredChanged(false);
    }
}
