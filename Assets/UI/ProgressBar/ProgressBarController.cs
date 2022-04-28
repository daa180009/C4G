using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    private float maximum;
    public float Maximum
    {
        set
        {
            maximum = value;
            updateDisplay();
        }
    }

    private float amount;
    public float Amount
    {
        set
        {
            amount = value;
            updateDisplay();
        }
    }

    void updateDisplay()
    {
        Image progressBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        progressBar.fillAmount = amount / maximum;
    }
}
