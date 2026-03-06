using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingUIItem : MonoBehaviour
{
    public int endingID;

    public GameObject discoveredObject;   // GameObject (adý görünen)
    public GameObject undiscoveredObject; // Undiscovered

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (EndingManager.IsEndingUnlocked(endingID))
        {
            discoveredObject.SetActive(true);
            undiscoveredObject.SetActive(false);
        }
        else
        {
            discoveredObject.SetActive(false);
            undiscoveredObject.SetActive(true);
        }
    }
}
