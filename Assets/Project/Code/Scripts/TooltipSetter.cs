using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI objectDescription;
    [SerializeField] private TextMeshProUGUI objectCost;

    public void SetTooltip(string objectName, string objectDescription = null, string objectCost = null)
    {
        Debug.Log("Set tooltip");

        this.objectName.text = objectName;

        if (objectDescription != null)
            this.objectDescription.text = objectDescription;

        if(objectCost != null)
            this.objectCost.text = objectCost;
    }
}
