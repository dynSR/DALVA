using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public delegate void ShopActionsHandler(int value);
    public event ShopActionsHandler OnBuyingAnItem;
    public event ShopActionsHandler OnSellingAnItem;
    public event ShopActionsHandler OnShopActionCancel;

    public Transform Player => GetComponentInParent<PlayerHUDManager>().Player;

    [Header("SHOP ACTIONS MADE")]
    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    private bool inventoryItemIsSelected = false;
    private bool shopItemIsSelected = false;
    private int numberOfShopActionsDone = 0;

    [Header("SHOP BOXES ICON")]
    [SerializeField] private List<ShopIcon> shopBoxesIcon;

    private CharacterRessources PlayerRessources => Player.GetComponent<CharacterRessources>();
    private EntityStats PlayerStats => Player.GetComponent<EntityStats>();
    public InventoryManager PlayerInventory { get => PlayerRessources.PlayerInventory;  }
    public InventoryBox SelectedInventoryBox { get; set; }
    public bool InventoryItemIsSelected { get => inventoryItemIsSelected; set => inventoryItemIsSelected = value; }
    public bool ShopItemIsSelected { get => shopItemIsSelected; set => shopItemIsSelected = value; }
    public Item SelectedItem { get; set; }
    public List<ShopIcon> ShopBoxesIcon { get => shopBoxesIcon; }

    [System.Serializable]
    public class ShopActionData
    {
        public string shopActionDataName;
        public enum ShopActionType { Purchase, Sale }
        public ShopActionType shopActionType;
        public Item item;
        public int itemValue;
        public int transactionID;

        public ShopActionData(string shopActionDataName ,ShopActionType shopActionType, Item item = null, int itemValue = 0, int transactionID = 0)
        {
            this.shopActionDataName = shopActionDataName;
            this.shopActionType = shopActionType;
            this.item = item;
            this.itemValue = itemValue;
            this.transactionID = transactionID;
        }
    }

    #region Buy an item
    public void AddShopActionOnPurchase(InventoryBox inventoryBoxItem = null, Item item = null)
    {
        numberOfShopActionsDone++;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        if(inventoryBoxItem != null)
        {
            string shopActionDataName = "Purchase " + inventoryBoxItem.StoredItem.ItemName;

            inventoryBoxItem.StoredItemTransactionID = numberOfShopActionsDone;

            AddShopActionData(shopActionDataName, ShopActionData.ShopActionType.Purchase, inventoryBoxItem);

            OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - inventoryBoxItem.StoredItem.ItemCost);
        }

        if(item != null) // Item Ability
        {
            string shopActionDataName = "Purchase " + item.ItemName;

            AddShopActionData(shopActionDataName, ShopActionData.ShopActionType.Purchase);

            OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - item.ItemCost);

            RefreshShopData();
        }
    }
    #endregion

    #region Used on shop buttons
    //Its on a button
    public void BuyItem(Item shopItem)
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase || !CanPurchaseItem(shopItem)) return;

        if (UtilityClass.RightClickIsPressed())
        {
            if(!shopItem.ItemIsAnAbility)
            {
                if (PlayerInventory.InventoryIsFull || IsItemAlreadyInInventory(shopItem)) return;

                PlayerInventory.AddItemToInventory(shopItem, true);
            }
            else
            {
                shopItem.EquipItemAsAbility(PlayerStats.EntityAbilities[shopItem.AbilityIndex]);
                AddShopActionOnPurchase(null, shopItem);
            }

            Debug.Log("Buying item : " + shopItem.ItemName);
        }
    }

    //Its on a button
    public void BuySelectedItemOnClickingOnButton()
    {
        if (!CanPurchaseItem(SelectedItem)) return;

        if (!SelectedItem.ItemIsAnAbility)
        {
            if (PlayerInventory.InventoryIsFull || IsItemAlreadyInInventory(SelectedItem)) return;

            PlayerInventory.AddItemToInventory(SelectedItem, true);
        }
        else
        {
            SelectedItem.EquipItemAsAbility(PlayerStats.EntityAbilities[SelectedItem.AbilityIndex]);
            AddShopActionOnPurchase(null, SelectedItem);
        }

        Debug.Log("Buying item : " + SelectedItem.ItemName);
    }

    //Its on a button
    public void SellItem()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase || PlayerInventory.InventoryIsEmpty || !InventoryItemIsSelected || SelectedInventoryBox.StoredItem == null) return;

        Debug.Log("Selling item : " + SelectedInventoryBox.StoredItem.ItemName);

        ShopActionOnSell(SelectedInventoryBox);
    }
    #endregion

    #region Sell an item
    public void ShopActionOnSell(InventoryBox inventoryBoxItem = null)
    {
        string shopActionDataName = "Sale " + inventoryBoxItem.StoredItem.ItemName;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        AddShopActionData(shopActionDataName, ShopActionData.ShopActionType.Sale, inventoryBoxItem);

        OnSellingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + inventoryBoxItem.StoredItem.AmountOfGoldRefundedOnSale);

        if (inventoryBoxItem != null)
        {
            PlayerInventory.RemoveItemFromInventory(inventoryBoxItem);
            PlayerInventory.ResetAllSelectedIcons();
        }
    }

    private void AddShopActionData(string shopActionDataName, ShopActionData.ShopActionType shopActionType, InventoryBox inventoryBoxItem = null)
    {
        if (inventoryBoxItem != null)
        {
            shopActions.Add(new ShopActionData(
            shopActionDataName,
            shopActionType,
            inventoryBoxItem.StoredItem,
            inventoryBoxItem.StoredItem.ItemCost,
            inventoryBoxItem.StoredItemTransactionID));
        }
        else
        {
           shopActions.Add(new ShopActionData(
           shopActionDataName,
           shopActionType));
        }
        
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

                            OnShopActionCancel?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + shopActions[i].item.ItemCost);

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

    public void RefreshShopData()
    {
        for (int i = 0; i < ShopBoxesIcon.Count; i++)
        {
            if (!ShopBoxesIcon[i].isActiveAndEnabled) continue;

            Item item = ShopBoxesIcon[i].ItemButton.ButtonItem;

            if (!item.ItemIsAnAbility)
            {
                Debug.Log(item.name + " " + item.ItemIsAnAbility);

                if (!CanPurchaseItem(item))
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                }
                else if (IsItemAlreadyInInventory(item))
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                    ShopBoxesIcon[i].ItemButton.DisplayCheckMark();
                    ShopBoxesIcon[i].ItemButton.DisplayPadlock();
                }
                else if (!IsItemAlreadyInInventory(item) && CanPurchaseItem(item))
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsDisponible();
                    ShopBoxesIcon[i].ItemButton.HideCheckMark();
                    ShopBoxesIcon[i].ItemButton.HidePadlock();
                }
            }

            if (item.ItemIsAnAbility)
            {
                AbilityLogic ability = PlayerStats.EntityAbilities[item.AbilityIndex];

                Debug.Log(PlayerStats.EntityAbilities[item.AbilityIndex].name);
                Debug.Log(item.name + " " + item.ItemIsAnAbility);

                if (ability.AbilitiesCooldownHandler.IsAbilityOnCooldown(ability) || !CanPurchaseItem(ShopBoxesIcon[i].ItemButton.ButtonItem))
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                }
                else if (!ability.AbilitiesCooldownHandler.IsAbilityOnCooldown(ability) && CanPurchaseItem(ShopBoxesIcon[i].ItemButton.ButtonItem))
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsDisponible();
                    ShopBoxesIcon[i].ItemButton.HideCheckMark();
                    ShopBoxesIcon[i].ItemButton.HidePadlock();
                }
                ////Si already equipped
                //else if (!CanPurchaseItem(shopBoxesIcon[i].ItemButton.ButtonItem))
                //{
                //    shopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                //    shopBoxesIcon[i].ItemButton.DisplayCheckMark();
                //    shopBoxesIcon[i].ItemButton.DisplayPadlock();
                //}
            }
        }
    }

    public void ResetSelectionIcon(bool ignoreCurrentObject = false)
    {
        for (int i = 0; i < ShopBoxesIcon.Count; i++)
        {
            if (ignoreCurrentObject && EventSystem.current.currentSelectedGameObject == ShopBoxesIcon[i].gameObject) continue;

            ShopBoxesIcon[i].ResetSelection();
            ShopBoxesIcon[i].ToggleOff();
        }
    }

    public bool IsItemAlreadyInInventory(Item item)
    {
        for (int i = PlayerInventory.InventoryBoxes.Count - 1; i >= 0; i--)
        {
            if (PlayerInventory.InventoryBoxes[i].StoredItem == item)
            {
                Debug.Log(item + " is already in inventory");
                return true;
            }
        }

        return false;
    }

    public bool CanPurchaseItem(Item item)
    {
        bool canPurchaseItem;

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