using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Shop shop;

    [SerializeField] private List<InventoryBox> inventoryBoxes;
    [SerializeField] private List<ToggleSelectionIcon> boxesToggleSelectionIcons;

    public bool InventoryIsFull => NumberOfFullInventoryBoxes >= InventoryBoxes.Count;
    public bool InventoryIsEmpty=> NumberOfFullInventoryBoxes <= 0;

    public GameObject LastInventoryBox { get; set; }
    public GameObject NewInventoryBox { get; set; }
    public int NumberOfFullInventoryBoxes { get; set; } = 0;
    public List<InventoryBox> InventoryBoxes { get => inventoryBoxes; }
    public List<ToggleSelectionIcon> BoxesToggleSelectionIcons { get => boxesToggleSelectionIcons; set => boxesToggleSelectionIcons = value; }
    public Shop Shop { get => shop; set => shop = value; }

    public void AddItemToInventory(Item itemToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (InventoryIsFull) return;

            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].UpdateInventoryBoxItem(itemToAdd, itemToAdd.ItemIcon);

                Debug.Log("Add " + itemToAdd.ItemName +" to inventory");
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    public void RemoveItemFromInventory(InventoryBox inventoryBox)
    {
        if (InventoryIsEmpty) return;

        inventoryBox.ResetInventoryBoxItem(inventoryBox);
        NumberOfFullInventoryBoxes--;
        Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
    }

    public void AddPurchasedItemToInventory(Item itemPurchasedToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (InventoryIsFull) return;

            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].UpdateInventoryBoxItem(itemPurchasedToAdd, itemPurchasedToAdd.ItemIcon);
                Shop.OnBuyingItem(InventoryBoxes[i]);

                Debug.Log("Add " + itemPurchasedToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    public void PlaceItemHere()
    {
        UpdateNewInventoryBox();
        ResetLastInventoryBox();
        ResetSelectionIcons();
    }

    void UpdateNewInventoryBox()
    {
        Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().StoredItem;
        Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;
        int _transactionID = LastInventoryBox.GetComponent<InventoryBox>().TransactionID;

        NewInventoryBox.GetComponent<InventoryBox>().UpdateInventoryBoxItem(lastInventoryBoxItem, lastInventoryBoxSprite);
        NewInventoryBox.GetComponent<InventoryBox>().TransactionID = _transactionID;
    }

    private void ResetLastInventoryBox()
    {
        InventoryBox lastInventoryBox = LastInventoryBox.GetComponent<InventoryBox>();
        lastInventoryBox.ResetInventoryBoxItem(lastInventoryBox);
    }

    public void SwapInventoryBoxesItem()
    {
        Debug.Log("Swap Inventory box item");

        Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().StoredItem;
        Item newInventoryBoxItem = NewInventoryBox.GetComponent<InventoryBox>().StoredItem;

        Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;
        Sprite newInventoryBoxSprite = NewInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;

        int lastInventoryBoxTransactionID = LastInventoryBox.GetComponent<InventoryBox>().TransactionID;
        int newInventoryBoxTransactionID = NewInventoryBox.GetComponent<InventoryBox>().TransactionID;

        //--- Last
        //Item
        LastInventoryBox.GetComponent<InventoryBox>().StoredItem = newInventoryBoxItem;
        //Icon
        LastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = newInventoryBoxSprite;
        //TransactionID
        LastInventoryBox.GetComponent<InventoryBox>().TransactionID = newInventoryBoxTransactionID;

        //--- New
        //Item
        NewInventoryBox.GetComponent<InventoryBox>().StoredItem = lastInventoryBoxItem;
        
        //Icon
        NewInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxSprite;
        
        //TransactionID
        NewInventoryBox.GetComponent<InventoryBox>().TransactionID = lastInventoryBoxTransactionID;

        ResetSelectionIcons();
    }

    public void ResetSelectionIcons()
    {
        for (int i = 0; i < BoxesToggleSelectionIcons.Count; i++)
        {
            BoxesToggleSelectionIcons[i].ToggleOff();
        }
    }
}