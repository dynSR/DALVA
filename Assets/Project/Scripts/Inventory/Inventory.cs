using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject lastInventoryBox;
    [SerializeField] private GameObject newInventoryBox;

    [SerializeField] private List<InventoryBox> inventoryBoxes;

    public GameObject LastInventoryBox { get => lastInventoryBox; set => lastInventoryBox = value; }
    public GameObject NewInventoryBox { get => newInventoryBox; set => newInventoryBox = value; }

    public void AddItemToInventory(Item itemToAdd)
    {
        Debug.Log("Add item to inventory");
        for (int i = 0; i < inventoryBoxes.Count; i++)
        {
            if (inventoryBoxes[i].InventoryBoxItem == null)
            {
                inventoryBoxes[i].UpdateInventoryBoxItem(itemToAdd);
                Debug.Log(inventoryBoxes.Count);
                return;
            }
        }
    }

    public void RemoveItemFromInventory(Item itemToRemove)
    {

    }

    public void PlaceItemHere()
    {
        UpdateNewInventoryBox();
        ResetLastInventoryBox();
    }

    public void SwapInventoryBoxesItem()
    {
        Debug.Log("Swap Inventory box item");

        Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem;
        Item newInventoryBoxItem = NewInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem;

        Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem.ItemIcon;
        Sprite newInventoryBoxSprite = NewInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem.ItemIcon;

        LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem = newInventoryBoxItem;
        LastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = newInventoryBoxSprite;

        NewInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem = lastInventoryBoxItem;
        NewInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxSprite;

        //Sprite newInventoryBoxItemSprite = NewInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon;
        //Sprite lastInventoryBoxItemSprite = LastInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon;

        ////Swap last with new sprite
        //LastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = newInventoryBoxItemSprite;

        ////Swap new with last sprite
        //NewInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxItemSprite;

        ////Set last icon with new
        //LastInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon = newInventoryBoxItemSprite;

        ////Set last icon with new
        //NewInventoryBox.transform.GetChild(0).GetComponent<DragIcon>().ItemIcon = lastInventoryBoxItemSprite;
    }

    void UpdateNewInventoryBox()
    {
        Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem;

        NewInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem = lastInventoryBoxItem;

        Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem.ItemIcon;

        NewInventoryBox.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        NewInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxSprite;
    }

    private void ResetLastInventoryBox()
    {
        LastInventoryBox.GetComponent<InventoryBox>().InventoryBoxItem = null;

        LastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = null;
        LastInventoryBox.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
    }
}
