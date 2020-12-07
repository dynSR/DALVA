using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private GameObject lastInventoryBox;
    [SerializeField] private GameObject newInventoryBox;

    [SerializeField] private List<InventoryBox> inventoryBoxes;

    public bool InventoryIsFull => NumberOfFullInventoryBoxes >= InventoryBoxes.Count;

    public GameObject LastInventoryBox { get => lastInventoryBox; set => lastInventoryBox = value; }
    public GameObject NewInventoryBox { get => newInventoryBox; set => newInventoryBox = value; }
    public int NumberOfFullInventoryBoxes { get; set; } = 0;
    public List<InventoryBox> InventoryBoxes { get => inventoryBoxes; }

    public void AddItemToInventory(Item itemToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (NumberOfFullInventoryBoxes >= InventoryBoxes.Count) return;

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

    public void RemoveItemFromInventory(Item itemToRemove)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (NumberOfFullInventoryBoxes <= 0) return;

            if (InventoryBoxes[i].StoredItem == itemToRemove)
            {
                InventoryBoxes[i].ResetInventoryBoxItem(InventoryBoxes[i]);
                NumberOfFullInventoryBoxes--;
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    public void AddPurchasedItemToInventory(Item itemPurchasedToAdd)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (NumberOfFullInventoryBoxes >= InventoryBoxes.Count) return;

            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].UpdateInventoryBoxItem(itemPurchasedToAdd, itemPurchasedToAdd.ItemIcon);
                shop.OnBuyingItem(InventoryBoxes[i]);

                Debug.Log("Add " + itemPurchasedToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    //PLACING
    public void PlaceItemHere()
    {
        UpdateNewInventoryBox();
        ResetLastInventoryBox();
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

    //SWAP
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
    }
}