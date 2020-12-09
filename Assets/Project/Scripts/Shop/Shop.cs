using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Inventory playerInventory;

    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    private int numberOfShopActionsDone = 0;

    public Inventory PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public Transform Player { get => player; set => player = value; }

    [System.Serializable]
    public class ShopActionData
    {
        public enum ShopActionType { Purchasing, Selling }
        public ShopActionType shopActionType;
        public int transactionID;

        public ShopActionData(ShopActionType shopActionType, int transactionID)
        {
            this.shopActionType = shopActionType;
            this.transactionID = transactionID;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetShopActions();
    }

    public void OnBuyingItem(InventoryBox inventoryBoxOfItemPurchased)
    {
        numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        inventoryBoxOfItemPurchased.TransactionID = numberOfShopActionsDone;

        shopActions.Add(new ShopActionData(ShopActionData.ShopActionType.Purchasing, inventoryBoxOfItemPurchased.TransactionID));
    }

    public void ResetShopActions()
    {
        shopActions.Clear();
        numberOfShopActionsDone = 0;
    }

    public void UndoShopAction()
    {
        if (numberOfShopActionsDone > 0)
        {
            for (int i = 0; i < PlayerInventory.InventoryBoxes.Count; i++)
            {
                if (PlayerInventory.InventoryBoxes[i].TransactionID == numberOfShopActionsDone)
                {
                    Debug.Log(PlayerInventory.InventoryBoxes[i].TransactionID + " / " + numberOfShopActionsDone);
                    Debug.Log(PlayerInventory.InventoryBoxes[i].name);

                    for (int j = shopActions.Count - 1; j >= 0; j--)
                    {
                        if (shopActions[j].transactionID == numberOfShopActionsDone)
                        {
                            if (shopActions[j].shopActionType == ShopActionData.ShopActionType.Purchasing)
                            {
                                PlayerInventory.InventoryBoxes[i].ResetInventoryBoxItem(PlayerInventory.InventoryBoxes[i]);
                            }
                            else  if (shopActions[j].shopActionType == ShopActionData.ShopActionType.Selling)
                            {
                                //Re Add to Inventory
                            }

                            shopActions.RemoveAt(j);
                        }
                    }

                    numberOfShopActionsDone--;
                    PlayerInventory.NumberOfFullInventoryBoxes--;
                    return;
                }
            }
        }
    }
}
