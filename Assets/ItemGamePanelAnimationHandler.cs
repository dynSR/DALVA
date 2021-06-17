using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGamePanelAnimationHandler : MonoBehaviour
{
    public GameObject[] objectsToActivateEarly;
    public GameObject[] objectsToActivate;

    public void ActivateObjects()
    {
        foreach (GameObject gO in objectsToActivate)
        {
            if (!gO.activeInHierarchy)
                gO.SetActive(true);
            else
            {
                gO.SetActive(false);
                gO.SetActive(true);
            }
        }
    }

    public void ActivateObjectsEarly()
    {
        foreach (GameObject gO in objectsToActivateEarly)
        {
            if (!gO.activeInHierarchy)
                gO.SetActive(true);
            else
            {
                gO.SetActive(false);
                gO.SetActive(true);
            }
        }
    }
}
