using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public PlayerResourceManager playerResourceManager;

    public void FixedUpdate()
    {
        foreach(Transform child in transform)
        {
            ManaCountController manaCountController = child.gameObject.GetComponent<ManaCountController>();
            int resourceCount = playerResourceManager.ManaTotal[manaCountController.manaType];

            if(resourceCount == 0)
            {
                manaCountController.gameObject.SetActive(false);
            }
            else
            {
                manaCountController.gameObject.SetActive(true);
                manaCountController.SetCount(resourceCount);
            }
        }
    }
}
