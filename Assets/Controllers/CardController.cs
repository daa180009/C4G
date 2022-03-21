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
    public CostIcon CostIconPrefab;

    public Text CardName;
    public Text TypeLine;
    public Text CardDescription;

    public GameObject CardCost;
    public UICollider uiCollider;

    public Image cardBack;
    public Image cardGlow;
    public Image cardGlowInner;
    public Image cardBorder;

    public WorldInfo worldInfo;

    CardData data;
    /// <summary>
    /// The card that is being visually represented
    /// </summary>
    public CardData Data
    {
        get { return data; }
        set
        {
            data = value;
            visualUpdate();
        }
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
    public float TargetGlowAlpha = 0f;
    float targetGlowAlphaSpeed = 0;
    float glowAlpha = 0f;

    /// <summary>
    /// The colour of the card;s border
    /// </summary>
    public Color TargetBorderColor = new Color(0, 0, 0);
    float targetBorderColorSpeed = 0;

    // used to make the glow of the card change over time for a nice visual effect
    float glowTimer = 0;
    float glowTimerInner = 0;

    /// <summary>
    /// The width (before scaling) of the card
    /// </summary>
    public float Width = 112f;
    /// <summary>
    /// The height (before scaling) of the card
    /// </summary>
    public float Height {
        get
        {
            return Width * 1.45f;
        }
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
    /// Register a function to be called when the useer stops hovering over this card
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
        uiCollider.RegisterPointerEntered(OnHover);
        uiCollider.RegisterPointerExited(OnUnhover);
        uiCollider.RegisterClicked(OnClick);
    }

    /// <summary>
    /// Updates the card to match the model's data
    /// </summary>
    void visualUpdate()
    {
        CardName.text = data.CardTitle;
        CardDescription.text = data.GetDescription(worldInfo);
        TypeLine.text = data.GetTypeLine();

        CardCost.transform.Clear();

        int addedIconsTotal = 0;
        foreach (KeyValuePair<Mana.ManaType, int> entry in data.ManaCostDictionary)
        {
            if(entry.Value >= 1)
                cardBack.color = CardData.GetColorOfManaType(entry.Key).AdjustedBrightness(.8f);

            for (int i = 0; i < entry.Value; i++)
            {
                CostIcon newIcon = Instantiate(CostIconPrefab, CardCost.transform.position, Quaternion.identity);
                newIcon.Type = entry.Key;
                newIcon.transform.SetParent(CardCost.transform);
                newIcon.transform.localPosition = new Vector3(-newIcon.GetComponent<RectTransform>().rect.width * (data.ManaValue - addedIconsTotal - 1), 0, 0);
                newIcon.transform.eulerAngles = currentEulerAngles;

                addedIconsTotal++;
            }
        }
    }

    void FixedUpdate()
    {
        if (doubleClickedTimer > 0)
            doubleClickedTimer--;

        RectTransform cardTransform = GetComponent<RectTransform>();

        // set horizontal position with damping
        if(horizontalEdge == RectTransform.Edge.Left)
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.SmoothDamp(cardTransform.offsetMin.x, TargetX, ref targetXSpeed, .15f), Width);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, Mathf.SmoothDamp(-cardTransform.offsetMax.x, TargetX, ref targetXSpeed, .15f), Width);

        // set vertical position with damping
        if (verticalEdge == RectTransform.Edge.Bottom)
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(cardTransform.offsetMin.y, TargetY, ref targetYSpeed, .15f), Height);
        else
            cardTransform.SetInsetAndSizeFromParentEdge(verticalEdge, Mathf.SmoothDamp(-cardTransform.offsetMax.y, TargetY, ref targetYSpeed, .15f), Height);

        // set rotation with damping
        currentEulerAngles = new Vector3(cardTransform.eulerAngles.x, cardTransform.eulerAngles.y, Mathf.SmoothDamp(currentEulerAngles.z, TargetRotation, ref targetRotationSpeed, .15f));
        cardTransform.eulerAngles = currentEulerAngles;

        // set scales with damping
        if(TargetScaleX != 0 && TargetScaleY != 0)
            cardTransform.SetGlobalScale(new Vector3(Mathf.SmoothDamp(cardTransform.lossyScale.x, TargetScaleX, ref targetScaleXSpeed, .15f), Mathf.SmoothDamp(cardTransform.lossyScale.y, TargetScaleY, ref targetScaleYSpeed, .15f), 1));

        // just set the alpha to a float and use it afterwards
        glowAlpha = Mathf.SmoothDamp(glowAlpha, TargetGlowAlpha, ref targetGlowAlphaSpeed, .15f);

        // set the alpha of the outer glow (the blue)
        glowTimer += 1f + UnityEngine.Random.Range(0f, 1f);
        if (glowTimer > 200f)
            glowTimer = 0;
        cardGlow.color = cardGlow.color.WithAlpha((Math.Abs(glowTimer - 100f) / 100 * .25f + .75f) * glowAlpha);

        // set the alpha of the inner glow (the white)
        glowTimerInner += 1f + UnityEngine.Random.Range(0f, 1f);
        if (glowTimerInner > 200f)
            glowTimerInner = 0;
        cardGlowInner.color = cardGlowInner.color.WithAlpha((Math.Abs(glowTimerInner - 100f) / 100 * .25f + .75f) * glowAlpha);

        // set the color of the border of the card
        cardBorder.color = cardBorder.color.SmoothDamp(TargetBorderColor, ref targetBorderColorSpeed, .01f);
    }

    public void OnHover()
    {
        if(cbHovered != null)
            cbHovered(this);
    }

    public void OnUnhover()
    {
        if(cbUnhovered != null)
            cbUnhovered(this);
    }

    public void OnClick()
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
