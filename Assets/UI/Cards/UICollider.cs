using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// for when you want only a specific part of the UI element to be clickable/hoverable 
public class UICollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Action cbPointerEntered;
    public void RegisterPointerEntered(Action cb) { cbPointerEntered -= cb; cbPointerEntered += cb; }
    public void UnregisterPointerEntered(Action cb) { cbPointerEntered -= cb; }

    Action cbPointerExited;
    public void RegisterPointerExited(Action cb) { cbPointerExited -= cb; cbPointerExited += cb; }
    public void UnregisterPointerExited(Action cb) { cbPointerExited -= cb; }

    Action cbClicked;
    public void RegisterClicked(Action cb) { cbClicked -= cb; cbClicked += cb; }
    public void UnregisterClicked(Action cb) { cbClicked -= cb; }

    Action cbRightClicked;
    public void RegisterRightClicked(Action cb) { cbRightClicked -= cb; cbRightClicked += cb; }
    public void UnregisterRightClicked(Action cb) { cbRightClicked -= cb; }

    Action cbMiddleClicked;
    public void RegisterMiddleClicked(Action cb) { cbMiddleClicked -= cb; cbMiddleClicked += cb; }
    public void UnregisterMiddleClicked(Action cb) { cbMiddleClicked -= cb; }

    //bool mousePresent = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cbPointerEntered != null)
            cbPointerEntered();

        //mousePresent = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cbPointerExited != null)
            cbPointerExited();

        //mousePresent = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (cbClicked != null)
                    cbClicked();
                break;
            case PointerEventData.InputButton.Right:
                if (cbRightClicked != null)
                    cbRightClicked();
                break;
            case PointerEventData.InputButton.Middle:
                if (cbMiddleClicked != null)
                    cbMiddleClicked();
                break;
        }
    }

    public void Update()
    {
    }
}
