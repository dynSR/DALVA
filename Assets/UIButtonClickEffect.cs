using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonClickEffect : MonoBehaviour, IPointerDownHandler
{
    public GameObject clickEffectToActivate;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickEffectToActivate == null) return;

        if (!clickEffectToActivate.activeInHierarchy)
            clickEffectToActivate.SetActive(true);
        else
        {
            clickEffectToActivate.SetActive(false);
            clickEffectToActivate.SetActive(true);
        }
    }
}