using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBox : MonoBehaviour
{
    public Item InventoryBoxItem; /*{ get; set; }*/
    private Image ItemIcon => transform.GetChild(0).GetComponent<Image>();
    private CanvasGroup CanvasGrp => transform.GetChild(0).GetComponent<CanvasGroup>();

    private void Start()
    {
        //ItemIcon.enabled = false;
        CanvasGrp.alpha = 0;
    }

    public void UpdateInventoryBoxItem(Item itemToUpdate)
    {
        InventoryBoxItem = itemToUpdate;

        CanvasGrp.alpha = 1;
        //ItemIcon.enabled = true;
        ItemIcon.sprite = itemToUpdate.ItemIcon;
    }
}
