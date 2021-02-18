using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private Shop shop;

    [Header("INVENTORY BOXES AND BOXES SELECTION ICONS")]
    [SerializeField] private List<InventoryBox> inventoryBoxes;
    [SerializeField] private List<ToggleSelectionIcon> boxesSelectionIcons; 

    public bool InventoryIsFull => NumberOfFullInventoryBoxes >= InventoryBoxes.Count;
    public bool InventoryIsEmpty=> NumberOfFullInventoryBoxes <= 0;

    public GameObject LastInventoryBox { get; set; }
    public GameObject NewInventoryBox { get; set; }
    public int NumberOfFullInventoryBoxes { get; set; }
    public List<InventoryBox> InventoryBoxes { get => inventoryBoxes; }

   public List<ToggleSelectionIcon> BoxesSelectionIcons { get => boxesSelectionIcons; set => boxesSelectionIcons = value; }
    public Shop Shop { get => shop; set => shop = value; }

    #region Inventory - Shop Management

    //Function used to add item to inventory without taking a shop system in consideration
    public void AddItemToInventory(Item itemToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (InventoryIsFull) return;

            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].ChangeInventoryBoxStoredItem(itemToAdd, itemToAdd.ItemIcon);

                Debug.Log("Add " + itemToAdd.ItemName +" to inventory");
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    // Function used to remove an item from inventory
    public void RemoveItemFromInventory(InventoryBox inventoryBox)
    {
        if (InventoryIsEmpty) return;

        inventoryBox.ResetInventoryBoxStoredItem(inventoryBox);
        NumberOfFullInventoryBoxes--;
        Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
    }

    //Function used to add item to inventory taking a shop system in consideration
    public void AddPurchasedItemToInventory(Item itemPurchasedToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (InventoryIsFull) return;

            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].ChangeInventoryBoxStoredItem(itemPurchasedToAdd, itemPurchasedToAdd.ItemIcon);
                Shop.OnBuyingItem(InventoryBoxes[i]);

                Debug.Log("Add " + itemPurchasedToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    //Function that resets the state of all boxes selection icons that have been toggled on
    public void ResetAllBoxesSelectionIcons()
    {
        for (int i = 0; i < BoxesSelectionIcons.Count; i++)
        {
            BoxesSelectionIcons[i].ToggleOff();
        }
    }
    #endregion

    #region Functions used for item swapping in the inventory - Commented for the moment
    //public void PlaceItemHere()
    //{
    //    UpdateNewInventoryBox();
    //    ResetLastInventoryBox();
    //    ResetSelectionIcons();
    //}

    //void UpdateNewInventoryBox()
    //{
    //    Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().StoredItem;
    //    Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;
    //    int _transactionID = LastInventoryBox.GetComponent<InventoryBox>().TransactionID;

    //    NewInventoryBox.GetComponent<InventoryBox>().UpdateInventoryBoxItem(lastInventoryBoxItem, lastInventoryBoxSprite);
    //    NewInventoryBox.GetComponent<InventoryBox>().TransactionID = _transactionID;
    //}

    //private void ResetLastInventoryBox()
    //{
    //    InventoryBox lastInventoryBox = LastInventoryBox.GetComponent<InventoryBox>();
    //    lastInventoryBox.ResetInventoryBoxItem(lastInventoryBox);
    //}

    //public void SwapInventoryBoxesItem()
    //{
    //    Debug.Log("Swap Inventory box item");

    //    Item lastInventoryBoxItem = LastInventoryBox.GetComponent<InventoryBox>().StoredItem;
    //    Item newInventoryBoxItem = NewInventoryBox.GetComponent<InventoryBox>().StoredItem;

    //    Sprite lastInventoryBoxSprite = LastInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;
    //    Sprite newInventoryBoxSprite = NewInventoryBox.GetComponent<InventoryBox>().StoredItem.ItemIcon;

    //    int lastInventoryBoxTransactionID = LastInventoryBox.GetComponent<InventoryBox>().TransactionID;
    //    int newInventoryBoxTransactionID = NewInventoryBox.GetComponent<InventoryBox>().TransactionID;

    //    //--- Last
    //    //Item
    //    LastInventoryBox.GetComponent<InventoryBox>().StoredItem = newInventoryBoxItem;
    //    //Icon
    //    LastInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = newInventoryBoxSprite;
    //    //TransactionID
    //    LastInventoryBox.GetComponent<InventoryBox>().TransactionID = newInventoryBoxTransactionID;

    //    //--- New
    //    //Item
    //    NewInventoryBox.GetComponent<InventoryBox>().StoredItem = lastInventoryBoxItem;

    //    //Icon
    //    NewInventoryBox.transform.GetChild(0).GetComponent<Image>().sprite = lastInventoryBoxSprite;

    //    //TransactionID
    //    NewInventoryBox.GetComponent<InventoryBox>().TransactionID = lastInventoryBoxTransactionID;

    //    ResetSelectionIcons();
    //}
    #endregion
}