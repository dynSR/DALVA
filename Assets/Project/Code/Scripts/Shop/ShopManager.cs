using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public delegate void ShopActionsHandler(int value);
    public static event ShopActionsHandler OnBuyingAnItem;
    public static event ShopActionsHandler OnSellingAnItem;
    public static event ShopActionsHandler OnShopActionCancel;

    public Transform Player => GetComponentInParent<PlayerHUDManager>().Player;

    [Header("SHOP ACTIONS MADE")]
    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    private bool inventoryItemIsSelected = false;
    private bool shopItemIsSelected = false;
    private int numberOfShopActionsDone = 0;

    [Header("SHOP BOXES ICON")]
    [SerializeField] private List<ShopIcon> shopBoxesIcon;

    private CharacterRessources PlayerRessources => Player.GetComponent<CharacterRessources>();
    public InventoryManager PlayerInventory { get => PlayerRessources.PlayerInventory;  }
    public InventoryBox SelectedInventoryBox { get; set; }
    public bool InventoryItemIsSelected { get => inventoryItemIsSelected; set => inventoryItemIsSelected = value; }
    public bool ShopItemIsSelected { get => shopItemIsSelected; set => shopItemIsSelected = value; }
    public Item SelectedItem { get; set; }

    [System.Serializable]
    public class ShopActionData
    {
        public string shopActionDataName;
        public enum ShopActionType { Purchase, Sale }
        public ShopActionType shopActionType;
        public Item item;
        public int itemValue;
        public int transactionID;

        public ShopActionData(string shopActionDataName ,ShopActionType shopActionType, Item item, int itemValue, int transactionID)
        {
            this.shopActionDataName = shopActionDataName;
            this.shopActionType = shopActionType;
            this.item = item;
            this.itemValue = itemValue;
            this.transactionID = transactionID;
        }
    }

    #region Buy an item
    public void AddShopActionOnPurchase(InventoryBox inventoryBoxItem)
    {
        string shopActionDataName = "Purchase " + inventoryBoxItem.StoredItem.ItemName;

        numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        inventoryBoxItem.StoredItemTransactionID = numberOfShopActionsDone;

        shopActions.Add(new ShopActionData(
            shopActionDataName, 
            ShopActionData.ShopActionType.Purchase, 
            inventoryBoxItem.StoredItem, 
            inventoryBoxItem.StoredItem.ItemCost, 
            inventoryBoxItem.StoredItemTransactionID));

        OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - inventoryBoxItem.StoredItem.ItemCost);
    }
    #endregion

    #region Used on shop buttons
    //Its on a button
    public void BuyItem(Item shopItem)
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase) return;

        if (UtilityClass.RightClickIsPressed())
        {
            if (PlayerInventory.InventoryIsFull || !CanPurchaseItem(shopItem) || IsItemAlreadyInInventory(shopItem)) return;

            PlayerInventory.AddItemToInventory(shopItem, true);

            Debug.Log("Buying item : " + shopItem.ItemName);
        }
    }

    //Its on a button
    public void BuySelectedItemOnClickingOnButton()
    {
        if (PlayerInventory.InventoryIsFull || !CanPurchaseItem(SelectedItem) || IsItemAlreadyInInventory(SelectedItem)) return;

        PlayerInventory.AddItemToInventory(SelectedItem, true);
    }

    //Its on a button
    public void SellItem()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase || PlayerInventory.InventoryIsEmpty || !InventoryItemIsSelected || SelectedInventoryBox.StoredItem == null) return;

        Debug.Log("Selling item : " + SelectedInventoryBox.StoredItem.ItemName);

        RemoveShopActionOnSell(SelectedInventoryBox);
    }
    #endregion

    #region Sell an item
    public void RemoveShopActionOnSell(InventoryBox inventoryBoxItem)
    {
        string shopActionDataName = "Sale " + inventoryBoxItem.StoredItem.ItemName;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        shopActions.Add(new ShopActionData(
            shopActionDataName, 
            ShopActionData.ShopActionType.Sale, 
            inventoryBoxItem.StoredItem, 
            inventoryBoxItem.StoredItem.ItemCost, 
            inventoryBoxItem.StoredItemTransactionID));

        OnSellingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + inventoryBoxItem.StoredItem.AmountOfGoldRefundedOnSale);

        PlayerInventory.RemoveItemFromInventory(inventoryBoxItem);
        PlayerInventory.ResetAllSelectedIcons();
    }
    #endregion

    #region Undo a purchase or a sell
    //Its on a button
    public void UndoShopAction()
    {
        if (numberOfShopActionsDone > 0)
        {
            for (int i = shopActions.Count - 1; i >= 0; i--)
            {
                //Undo A Purchase
                if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Purchase)
                {
                    for (int j = 0; j < PlayerInventory.InventoryBoxes.Count; j++)
                    {
                        if (PlayerInventory.InventoryBoxes[j].StoredItemTransactionID == numberOfShopActionsDone)
                        {
                            //Debug.Log(PlayerInventory.InventoryBoxes[j].StoredItemTransactionID + " / " + numberOfShopActionsDone);
                            //Debug.Log(PlayerInventory.InventoryBoxes[j].name);
                            //Debug.Log("Last shop action is a purchase action");

                            OnShopActionCancel?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + PlayerInventory.InventoryBoxes[j].StoredItem.ItemCost);

                            PlayerInventory.RemoveItemFromInventory(PlayerInventory.InventoryBoxes[j]);

                            numberOfShopActionsDone--;

                            PlayerInventory.ResetAllSelectedIcons();
                            PlayerInventory.InventoryBoxes[j].StoredItemTransactionID = 0;

                            shopActions.RemoveAt(i);

                            return;
                        }
                    }
                }
                //Undo A Sell
                else if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Sale)
                {
                    Debug.Log("Last shop action is a sale action");

                    AddSoldItemToInventory(shopActions[i], shopActions[i].item, shopActions[i].transactionID);

                    PlayerInventory.ResetAllSelectedIcons();

                    shopActions.RemoveAt(i);
                    return;
                }
            }
        }
    }

    private void AddSoldItemToInventory(ShopActionData shopActionData, Item itemToAdd, int transactionIDData)
    {
        for (int i = 0; i < PlayerInventory.InventoryBoxes.Count; i++)
        {
            if (PlayerInventory.InventoryIsFull) return;

            if (PlayerInventory.InventoryBoxes[i].StoredItem == null
                && shopActionData.transactionID == PlayerInventory.InventoryBoxes[i].StoredItemTransactionID)
            {
                PlayerInventory.AddItemToInventory(itemToAdd);
                PlayerInventory.InventoryBoxes[i].StoredItemTransactionID = transactionIDData;

                OnShopActionCancel?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - itemToAdd.InventoryBox.StoredItem.AmountOfGoldRefundedOnSale);

                Debug.Log("Add " + itemToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + PlayerInventory.NumberOfFullInventoryBoxes);
                return;
            }
        }
    }
    #endregion

    void RefreshShopData()
    {
        //Lock les équipements qui ne sont plus achetables - check des ressources
        //Lock les équipements qui sont déjà dans l'inventaire après l'achat
    }

    public void ResetSelectionIcon()
    {
        for (int i = 0; i < shopBoxesIcon.Count; i++)
        {
            shopBoxesIcon[i].ResetSelection();
        }
    }

    public bool IsItemAlreadyInInventory(Item item)
    {
        bool isItemAlreadyInInventory = false;

        for (int i = PlayerInventory.InventoryBoxes.Count - 1; i >= 0; i--)
        {
            if (PlayerInventory.InventoryBoxes[i].StoredItem == item)
            {
                isItemAlreadyInInventory = true;
            }
            else isItemAlreadyInInventory = false;
        }

        return isItemAlreadyInInventory;
    }

    public bool CanPurchaseItem(Item item)
    {
        bool canPurchaseItem = false;

        if (Player.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources >= item.ItemCost)
        {
            canPurchaseItem = true;
        }
        else canPurchaseItem = false;

        return canPurchaseItem;
    }

    #region Debuging
    public void ResetShopActions() // Debug purpose
    {
        shopActions.Clear();
        numberOfShopActionsDone = 0;
    }
    #endregion
}