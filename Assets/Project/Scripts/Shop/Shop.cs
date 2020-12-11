using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryBox selectedInventoryBox;

    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    public int numberOfShopActionsDone = 0;

    public Inventory PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public Transform Player { get => player; set => player = value; }
    public InventoryBox SelectedInventoryBox { get => selectedInventoryBox; set => selectedInventoryBox = value; }

    [System.Serializable]
    public class ShopActionData
    {
        public string shopActionDataName;
        public enum ShopActionType { Purchase, Sale }
        public ShopActionType shopActionType;
        public Item item;
        public int transactionID;

        public ShopActionData(string shopActionDataName ,ShopActionType shopActionType, Item item,int transactionID)
        {
            this.shopActionDataName = shopActionDataName;
            this.shopActionType = shopActionType;
            this.item = item;
            this.transactionID = transactionID;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetShopActions();
    }

    #region Buy an item
    public void BuyItem(Item shopItem)
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerInventory.InventoryIsFull) return;

            PlayerInventory.AddPurchasedItemToInventory(shopItem);
            Debug.Log("Buying item : " + shopItem.ItemName);
        }
    }

    public void OnBuyingItem(InventoryBox inventoryBoxOfItemPurchased)
    {
        string shopActionDataName = "Purchase " + inventoryBoxOfItemPurchased.StoredItem.ItemName;

        numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        inventoryBoxOfItemPurchased.TransactionID = numberOfShopActionsDone;

        shopActions.Add(new ShopActionData(shopActionDataName, ShopActionData.ShopActionType.Purchase, inventoryBoxOfItemPurchased.StoredItem, inventoryBoxOfItemPurchased.TransactionID));
    }
    #endregion

    #region Sell an item
    public void SellItem()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase || PlayerInventory.InventoryIsEmpty) return;

        Debug.Log("Selling item : " + SelectedInventoryBox.StoredItem.ItemName);

        OnSellingItem(SelectedInventoryBox);
    }

    public void AddSoldItemToInventory(ShopActionData shopActionData, Item itemToAdd, int transactionIDData)
    {
        for (int i = 0; i < playerInventory.InventoryBoxes.Count; i++)
        {
            if (playerInventory.InventoryIsFull) return;

            if (playerInventory.InventoryBoxes[i].StoredItem == null && shopActionData.transactionID == playerInventory.InventoryBoxes[i].TransactionID)
            {
                playerInventory.NumberOfFullInventoryBoxes++;
                playerInventory.InventoryBoxes[i].UpdateInventoryBoxItem(itemToAdd, itemToAdd.ItemIcon);
                playerInventory.InventoryBoxes[i].TransactionID = transactionIDData;


                Debug.Log("Add " + itemToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + playerInventory.NumberOfFullInventoryBoxes);
                return;
            }
        }
    }

    public void OnSellingItem(InventoryBox inventoryBoxOfItemSold)
    {
        string shopActionDataName = "Sale " + inventoryBoxOfItemSold.StoredItem.ItemName;

        //numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        //inventoryBoxOfItemSold.TransactionID = numberOfShopActionsDone;

        shopActions.Add(new ShopActionData(shopActionDataName, ShopActionData.ShopActionType.Sale, inventoryBoxOfItemSold.StoredItem, inventoryBoxOfItemSold.TransactionID));

        PlayerInventory.RemoveItemFromInventory(inventoryBoxOfItemSold);
        PlayerInventory.ResetSelectionIcons();
    }
    #endregion

    public void ResetShopActions()
    {
        shopActions.Clear();
        numberOfShopActionsDone = 0;
    }

    public void UndoShopAction()
    {
        if (numberOfShopActionsDone > 0)
        {
            for (int i = shopActions.Count - 1; i >= 0; i--)
            {
                if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Purchase)
                {
                    for (int j = 0; j < PlayerInventory.InventoryBoxes.Count; j++)
                    {
                        if (PlayerInventory.InventoryBoxes[j].TransactionID == numberOfShopActionsDone)
                        {
                            Debug.Log(PlayerInventory.InventoryBoxes[j].TransactionID + " / " + numberOfShopActionsDone);
                            Debug.Log(PlayerInventory.InventoryBoxes[j].name);

                            Debug.Log("Last shop action is a purchase action");
                            PlayerInventory.InventoryBoxes[j].ResetInventoryBoxItem(PlayerInventory.InventoryBoxes[j]);

                            numberOfShopActionsDone--;
                            PlayerInventory.NumberOfFullInventoryBoxes--;
                            playerInventory.ResetSelectionIcons();
                            PlayerInventory.InventoryBoxes[j].TransactionID = 0;
                            shopActions.RemoveAt(i);
                            return;
                        }
                    }
                }
                else if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Sale)
                {
                    Debug.Log("Last shop action is a sale action");
                    AddSoldItemToInventory(shopActions[i], shopActions[i].item, shopActions[i].transactionID);

                    playerInventory.ResetSelectionIcons();
                    shopActions.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
