using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// The UI element representing a card in the user's hand or otherwise viewable by the user
/// </summary>
public class CardController : MonoBehaviour
{
    public VisualCardController visualCard;

    public CardData Data
    {
        get
        {
            return visualCard.Data;
        }
        set
        {
            visualCard.Data = value;
        }
    }

    public float Width
    {
        get { return visualCard.Width; }
    }
    public float Height
    {
        get { return visualCard.Height; }
    }

    /// <summary>
    /// Which edge to align the card with horizontally
    /// </summary>
    public RectTransform.Edge horizontalEdge = RectTransform.Edge.Left;
    /// <summary>
    /// Which edge to align the card with vertically
    /// </summary>
    public RectTransform.Edge verticalEdge = RectTransform.Edge.Bottom;

    /// <summary>
    /// The x position to move the visual card to over time
    /// </summary>
    public float TargetX = 0f;
    float targetXSpeed = 0f;

    /// <summary>
    /// The y position (from the bottom of the parent) to move the visual card to over time
    /// </summary>
    public float TargetY = 0f;
    float targetYSpeed = 0f;

    /// <summary>
    /// The z-rotation to move the visual card to over time
    /// </summary>
    public float TargetRotation = 0f;
    float targetRotationSpeed = 0;
    Vector3 currentEulerAngles = Vector3.zero;
    
    /// <summary>
    /// The horizontal scale to change the visual card to over time
    /// </summary>
    public float TargetScaleX = 1f;
    float targetScaleXSpeed = 0;

    /// <summary>
    /// The vertical scale to change the visual card to over time
    /// </summary>
    public float TargetScaleY = 1f;
    float targetScaleYSpeed = 0;

    /// <summary>
    /// The alpha for the glowing border around the card
    /// </summary>
    public float TargetGlowAlpha {
        get { return visualCard.TargetGlowAlpha;  }
        set { visualCard.TargetGlowAlpha = value; }
    }

    /// <summary>
    /// The colour of the card's border
    /// </summary>
    public Color TargetBorderColor
    {
        get { return visualCard.TargetBorderColor; }
        set { visualCard.TargetBorderColor = value;  }
    }
    
    /// <summary>
    /// The ticks since the last time the card was clicked.
    /// </summary>
    public int doubleClickedTimer = 0;

    Action<CardController> cbHovered;
    /// <summary>
    /// Register a function to be called when the user hovers over this card
    /// </summary>
    public void RegisterHovered(Action<CardController> cb) { cbHovered -= cb; cbHovered += cb; }
    public void UnregisterHovered(Action<CardController> cb) { cbHovered -= cb; }

    Action<CardController> cbUnhovered;
    /// <summary>
    /// Register a function to be called when the user stops hovering over this card
    /// </summary>
    public void RegisterUnhovered(Action<CardController> cb) { cbUnhovered -= cb; cbUnhovered += cb; }
    public void UnregisterUnhovered(Action<CardController> cb) { cbUnhovered -= cb; }

    Action<CardController> cbPlayed;
    /// <summary>
    /// Register a function to be called when the user attempts to play this card
    /// </summary>
    public void RegisterPlayed(Action<CardController> cb) { cbPlayed -= cb; cbPlayed += cb; }
    public void UnregisterPlayed(Action<CardController> cb) { cbPlayed -= cb; }

    void Awake()
    {
        visualCard.RegisterHovered(OnHover);
        visualCard.RegisterUnhovered(OnUnhover);
        visualCard.RegisterClicked(OnClick);
    }

    void FixedUpdate()
    {
        if (doubleClickedTimer > 0)
            doubleClickedTimer--;

        RectTransform cardTransform = GetComponent<RectTransform>();

        // set horizontal position with damping
        if(horizontalEdge == RectTransform.Edge.Left)
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.SmoothDamp(cardTransform.offsetMin.x, TargetX, ref targetXSpeed, .15f), visualCard.Width);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, Mathf.SmoothDamp(-cardTransform.offsetMax.x, TargetX, ref targetXSpeed, .15f), visualCard.Width);

        // set vertical position with damping
        if (verticalEdge == RectTransform.Edge.Bottom)
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(cardTransform.offsetMin.y, TargetY, ref targetYSpeed, .15f), visualCard.Height);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(-cardTransform.offsetMax.y, TargetY, ref targetYSpeed, .15f), visualCard.Height);

        // set rotation with damping
        currentEulerAngles = new Vector3(cardTransform.eulerAngles.x, cardTransform.eulerAngles.y, Mathf.SmoothDamp(currentEulerAngles.z, TargetRotation, ref targetRotationSpeed, .15f));
        cardTransform.eulerAngles = currentEulerAngles;

        // set scales with damping
        if(TargetScaleX != 0 && TargetScaleY != 0)
            cardTransform.SetGlobalScale(new Vector3(Mathf.SmoothDamp(cardTransform.lossyScale.x, TargetScaleX, ref targetScaleXSpeed, .15f), Mathf.SmoothDamp(cardTransform.lossyScale.y, TargetScaleY, ref targetScaleYSpeed, .15f), 1));
    }

    public void OnHover(VisualCardController ownVisualCard)
    {
        if (cbHovered != null)
            cbHovered(this);
    }

    public void OnUnhover(VisualCardController ownVisualCard)
    {
        if (cbUnhovered != null)
            cbUnhovered(this);
    }

    public void OnClick(VisualCardController ownVisualCard)
    {
        if(doubleClickedTimer > 0)
        {
            if(cbPlayed != null)
                cbPlayed(this);
            doubleClickedTimer = 0;
        }
        else
        {
            doubleClickedTimer = 25;
        }
    }
}
