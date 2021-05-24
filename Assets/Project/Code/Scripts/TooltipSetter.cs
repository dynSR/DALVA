using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectName;
    [SerializeField] private TextMeshProUGUI objectDescription;
    [SerializeField] private TextMeshProUGUI objectCost;
    [SerializeField] private Image image;

    public void SetTooltip(string objectName, string objectDescription = null, string objectCost = null, Sprite icon = null)
    {
        Debug.Log("Set tooltip");

        this.objectName.text = objectName;

        if (objectDescription != null)
            this.objectDescription.text = objectDescription;

        if(objectCost != null)
            this.objectCost.text = objectCost;

        if (image != null && icon != null)
            image.sprite = icon;
    }
}
