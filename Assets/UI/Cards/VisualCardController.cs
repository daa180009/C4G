using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VisualCardController : MonoBehaviour
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
    /// The width (before scaling) of the card
    /// </summary>
    public float Width = 112f;
    /// <summary>
    /// The height (before scaling) of the card
    /// </summary>
    public float Height
    {
        get
        {
            return Width * 1.45f;
        }
    }

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

    Action<VisualCardController> cbHovered;
    /// <summary>
    /// Register a function to be called when the user hovers over this card
    /// </summary>
    public void RegisterHovered(Action<VisualCardController> cb) { cbHovered -= cb; cbHovered += cb; }
    public void UnregisterHovered(Action<VisualCardController> cb) { cbHovered -= cb; }

    Action<VisualCardController> cbUnhovered;
    /// <summary>
    /// Register a function to be called when the user stops hovering over this card
    /// </summary>
    public void RegisterUnhovered(Action<VisualCardController> cb) { cbUnhovered -= cb; cbUnhovered += cb; }
    public void UnregisterUnhovered(Action<VisualCardController> cb) { cbUnhovered -= cb; }

    Action<VisualCardController> cbClicked;
    /// <summary>
    /// Register a function to be called when the user attempts to play this card
    /// </summary>
    public void RegisterClicked(Action<VisualCardController> cb) { cbClicked -= cb; cbClicked += cb; }
    public void UnregisterClicked(Action<VisualCardController> cb) { cbClicked -= cb; }

    Action<VisualCardController> cbRightClicked;
    /// <summary>
    /// Register a function to be called when the user attempts to play this card
    /// </summary>
    public void RegisterRightClicked(Action<VisualCardController> cb) { cbRightClicked -= cb; cbRightClicked += cb; }
    public void UnregisterRightClicked(Action<VisualCardController> cb) { cbRightClicked -= cb; }

    void Awake()
    {
        uiCollider.RegisterPointerEntered(OnHover);
        uiCollider.RegisterPointerExited(OnUnhover);
        uiCollider.RegisterClicked(OnClick);
        uiCollider.RegisterRightClicked(OnRightClick);
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
            if (entry.Value >= 1)
                cardBack.color = CardData.GetColorOfManaType(entry.Key).AdjustedBrightness(.8f);

            for (int i = 0; i < entry.Value; i++)
            {
                CostIcon newIcon = Instantiate(CostIconPrefab, CardCost.transform.position, Quaternion.identity);
                newIcon.Type = entry.Key;
                newIcon.transform.SetParent(CardCost.transform);
                newIcon.transform.localPosition = new Vector3(-newIcon.GetComponent<RectTransform>().rect.width * (data.ManaValue - addedIconsTotal - 1), 0, 0);
                //newIcon.transform.eulerAngles = currentEulerAngles;

                addedIconsTotal++;
            }
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Height);
    }

    public void FixedUpdate()
    {
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
        if (cbHovered != null)
            cbHovered(this);
    }

    public void OnUnhover()
    {
        if (cbUnhovered != null)
            cbUnhovered(this);
    }

    public void OnClick()
    {
        if (cbClicked != null)
            cbClicked(this);
    }

    public void OnRightClick()
    {
        if (cbRightClicked != null)
            cbRightClicked(this);
    }
}
