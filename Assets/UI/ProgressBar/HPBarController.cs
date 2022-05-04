using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarController : MonoBehaviour
{
    public ProgressBarController bar;

    public float Maximum
    {
        set
        {
            bar.Maximum = value;
        }
    }

    public float Amount
    {
        set
        {
            bar.Amount = value;
        }
    }
}
