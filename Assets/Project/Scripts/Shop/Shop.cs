using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private List<int> shopActions = new List<int>();
    private int numberOfShopActionsDone = 0;

    public bool IsShopWindowOpened => transform.gameObject.activeInHierarchy;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetShopActions();
    }

    public void OnBuyingItem(InventoryBox inventoryBoxOfItemPurchased)
    {
        numberOfShopActionsDone++;
        Debug.Log("T.ID " + inventoryBoxOfItemPurchased.TransactionID);
        inventoryBoxOfItemPurchased.TransactionID = numberOfShopActionsDone;
        shopActions.Add(inventoryBoxOfItemPurchased.TransactionID);
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
            for (int i = 0; i < inventory.InventoryBoxes.Count; i++)
            {
                if (inventory.InventoryBoxes[i].TransactionID == numberOfShopActionsDone)
                {
                    Debug.Log(inventory.InventoryBoxes[i].TransactionID + " / " + numberOfShopActionsDone);
                    Debug.Log(inventory.InventoryBoxes[i].name);

                    inventory.InventoryBoxes[i].ResetInventoryBoxItem(inventory.InventoryBoxes[i]);

                    shopActions.Remove(shopActions.Count);

                    numberOfShopActionsDone--;
                    inventory.NumberOfFullInventoryBoxes--;
                    return;
                }
            }
        }
    }
}
