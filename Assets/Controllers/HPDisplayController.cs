using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPDisplayController : MonoBehaviour
{
    public PlayerResourceManager playerResourceManager;

    public TextMeshProUGUI HPCount;

    // Update is called once per frame
    void Update()
    {
        HPCount.text = playerResourceManager.LifeTotal.ToString() + " HP";
    }
}
