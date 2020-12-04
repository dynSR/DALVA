using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject lastInventoryBox;
    public GameObject newInventoryBox;

    public void SwapInventoryBoxItem()
    {
        Debug.Log("Swap Inventory box item");

        Sprite newInventoryBoxItemSprite = newInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon;
        Sprite lastInventoryBoxItemSprite = lastInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon;

        //Swap last with new sprite
        lastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = newInventoryBoxItemSprite;

        //Swap new with last sprite
        newInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxItemSprite;

        //Set last icon with new
        lastInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon = newInventoryBoxItemSprite;

        //Set last icon with new
        newInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon = lastInventoryBoxItemSprite;
    }
}
