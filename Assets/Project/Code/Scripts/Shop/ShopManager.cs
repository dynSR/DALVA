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

    [Header("PLAYER INFORMATIONS")]
    [SerializeField] private Transform player;
    private InventoryBox selectedInventoryBox;

    [Header("SHOP ACTIONS MADE")]
    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    private int numberOfShopActionsDone = 0;

    public Transform Player { get => player; set => player = value; }
    private CharacterRessources PlayerRessources => player.GetComponent<CharacterRessources>();
    public InventoryManager PlayerInventory { get => PlayerRessources.PlayerInventory;  }
    public InventoryBox SelectedInventoryBox { get => selectedInventoryBox; set => selectedInventoryBox = value; }

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetShopActions(); // Debug purpose
    }

    #region Buy an item
    //Its on a button
    public void BuyItem(Item shopItem)
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase) return;

        if (UtilityClass.RightClickIsPressed())
        {
            if (PlayerInventory.InventoryIsFull) return;

            PlayerInventory.AddPurchasedItemToInventory(shopItem);
            Debug.Log("Buying item : " + shopItem.ItemName);
        }
    }

    public void OnBuyingItem(InventoryBox inventoryBoxOfItem)
    {
        string shopActionDataName = "Purchase " + inventoryBoxOfItem.StoredItem.ItemName;

        numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        inventoryBoxOfItem.StoredItemTransactionID = numberOfShopActionsDone;

        shopActions.Add(new ShopActionData(
            shopActionDataName, 
            ShopActionData.ShopActionType.Purchase, 
            inventoryBoxOfItem.StoredItem, 
            inventoryBoxOfItem.StoredItem.ItemCost, 
            inventoryBoxOfItem.StoredItemTransactionID));

        OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - inventoryBoxOfItem.StoredItem.ItemCost);
    }
    #endregion

    #region Sell an item
    //Its on a button
    public void SellItem()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase || PlayerInventory.InventoryIsEmpty) return;

        Debug.Log("Selling item : " + SelectedInventoryBox.StoredItem.ItemName);

        OnSellingItem(SelectedInventoryBox);
    }

    public void OnSellingItem(InventoryBox inventoryBoxOfItem)
    {
        string shopActionDataName = "Sale " + inventoryBoxOfItem.StoredItem.ItemName;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        shopActions.Add(new ShopActionData(
            shopActionDataName, 
            ShopActionData.ShopActionType.Sale, 
            inventoryBoxOfItem.StoredItem, 
            inventoryBoxOfItem.StoredItem.ItemCost, 
            inventoryBoxOfItem.StoredItemTransactionID));

        OnSellingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + inventoryBoxOfItem.StoredItem.ItemCost);

        PlayerInventory.RemoveItemFromInventory(inventoryBoxOfItem);
        PlayerInventory.ResetAllBoxesSelectionIcons();
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
                            Debug.Log(PlayerInventory.InventoryBoxes[j].StoredItemTransactionID + " / " + numberOfShopActionsDone);
                            Debug.Log(PlayerInventory.InventoryBoxes[j].name);
                            Debug.Log("Last shop action is a purchase action");

                            PlayerInventory.InventoryBoxes[j].ResetInventoryBoxStoredItem(PlayerInventory.InventoryBoxes[j]);

                            numberOfShopActionsDone--;
                            PlayerInventory.NumberOfFullInventoryBoxes--;

                            PlayerInventory.ResetAllBoxesSelectionIcons();

                            PlayerInventory.InventoryBoxes[j].StoredItemTransactionID = 0;

                            shopActions.RemoveAt(i);

                            //OnShopActionCancelEvent?.Invoke(PlayerRessources.AmountOfPlayerRessources);

                            return;
                        }
                    }
                }
                //Undo A Sell
                else if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Sale)
                {
                    Debug.Log("Last shop action is a sale action");

                    AddSoldItemToInventory(shopActions[i], shopActions[i].item, shopActions[i].transactionID);

                    PlayerInventory.ResetAllBoxesSelectionIcons();

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

            if (PlayerInventory.InventoryBoxes[i].StoredItem == null && shopActionData.transactionID == PlayerInventory.InventoryBoxes[i].StoredItemTransactionID)
            {
                PlayerInventory.NumberOfFullInventoryBoxes++;
                PlayerInventory.InventoryBoxes[i].ChangeInventoryBoxStoredItem(itemToAdd, itemToAdd.ItemIcon);
                PlayerInventory.InventoryBoxes[i].StoredItemTransactionID = transactionIDData;


                Debug.Log("Add " + itemToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + PlayerInventory.NumberOfFullInventoryBoxes);
                return;
            }
        }
    }
    #endregion

    #region Debuging
    public void ResetShopActions() // Debug purpose
    {
        shopActions.Clear();
        numberOfShopActionsDone = 0;
    }
    #endregion
}