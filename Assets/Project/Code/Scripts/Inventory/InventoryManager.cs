using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private ShopManager shop;

    [Header("INVENTORY BOXES AND BOXES SELECTION ICONS")]
    [SerializeField] private List<InventoryBox> inventoryBoxes;
    [SerializeField] private List<SelectIcon> parentOfSelectedIcons;

    public GameObject highlightObject;

    public bool InventoryIsFull => NumberOfFullInventoryBoxes >= InventoryBoxes.Count;
    public bool InventoryIsEmpty=> NumberOfFullInventoryBoxes <= 0;

    public int NumberOfFullInventoryBoxes { get; set; }
    public List<InventoryBox> InventoryBoxes { get => inventoryBoxes; }
    public List<SelectIcon> ParentOfSelectedIcons { get => parentOfSelectedIcons; }
    public ShopManager Shop { get => shop; set => shop = value; }


    #region Inventory - Shop Management
    // Function used to remove an item from inventory
    public void RemoveItemFromInventory(InventoryBox inventoryBox)
    {
        if (InventoryIsEmpty) return;

        if (inventoryBox.StoredItem != null)
            inventoryBox.StoredItem.UnequipItemAsEquipement(Shop.Player.GetComponent<EntityStats>());

        NumberOfFullInventoryBoxes--;

        inventoryBox.ResetInventoryBoxStoredItem(inventoryBox);
        Shop.RefreshShopData();

        DisplayHighlight();

        //Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
    }

    //Function used to add item to inventory taking a shop system in consideration
    public void AddItemToInventory(Item item, bool hasBeenPurchased = false)
    {
        for (int i = 0; i < InventoryBoxes.Count; i++)
        {
            if (InventoryBoxes[i].StoredItem == null)
            {
                NumberOfFullInventoryBoxes++;
                InventoryBoxes[i].ChangeInventoryBoxStoredItem(item, item.ItemIcon);

                item.EquipItemAsEquipement(Shop.Player.GetComponent<EntityStats>());

                if (hasBeenPurchased)
                    Shop.AddShopActionOnPurchase(InventoryBoxes[i]);

                Shop.RefreshShopData();

                DisplayHighlight();

                //Debug.Log("Add " + item.ItemName + " to inventory");
                //Debug.Log("Number of full inventory boxes : " + NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    //Function that resets the state of all boxes selection icons that have been toggled on
    public void ResetAllSelectedIcons()
    {
        for (int i = 0; i < ParentOfSelectedIcons.Count; i++)
        {
            ParentOfSelectedIcons[i].ToggleOff();
        }
    }

    public void ResetSomeSelectedIcons()
    {
        int selectedIcons = 0;

        for (int i = ParentOfSelectedIcons.Count - 1; i >= 0; i--)
        {
            if (ParentOfSelectedIcons[i].IsSelected)
                selectedIcons++;

            if (selectedIcons >= 1)
            {
                foreach (var item in ParentOfSelectedIcons)
                {
                    item.ToggleOff();
                }
            }
        }
    }

    private void DisplayHighlight()
    {
        if(highlightObject.activeInHierarchy)
        {
            highlightObject.SetActive(false);
            highlightObject.SetActive(true);
        }
        else highlightObject.SetActive(true);
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