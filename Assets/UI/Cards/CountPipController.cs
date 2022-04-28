using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountPipController : MonoBehaviour
{
    public Image innerPip;

    bool isOn;
    public bool IsOn
    {
        get { return isOn; }
        set
        {
            isOn = value;
            if (isOn)
            {
                innerPip.color = Color.blue;
            }
            else
            {
                innerPip.color = Color.black;
            }
        }
    }
}
