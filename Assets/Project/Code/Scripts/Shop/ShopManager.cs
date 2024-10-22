﻿using DarkTonic.MasterAudio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public delegate void ShopActionsHandler(int value);
    public event ShopActionsHandler OnBuyingAnItem;
    public event ShopActionsHandler OnSellingAnItem;
    public event ShopActionsHandler OnShopActionCancel;
    public event ShopActionsHandler OnShopDrawSetResetCost;

    public delegate void ShopShuffleHandler();
    public event ShopShuffleHandler OnShuffleDone;
    public event ShopShuffleHandler OnShuffle;

    public Transform Player => transform.parent.transform.parent.transform.parent;

    [Header("SHOP ACTIONS MADE")]
    [SerializeField] private List<ShopActionData> shopActions = new List<ShopActionData>();
    private bool inventoryItemIsSelected = false;
    private bool shopItemIsSelected = false;
    private int numberOfShopActionsDone = 0;

    [Header("SHOP BOXES ICON")]
    [SerializeField] private List<ShopIcon> shopBoxesIcon;

    [Header("ITEMS IN POOL")]
    //[SerializeField] private bool canStackSameItem = false;
    [SerializeField] private bool shuffleItemsOnlyWhenFifthWaveIsOver = true;
    [SerializeField] private bool costScalesUP = false;
    [SerializeField] private int costAugmentation;
    [SerializeField] private int amntOfItemsToAddInThePool;
    //[HideInInspector]
    public List<ItemButton> itemCreated;
    public bool firstDrawDone = false;
    public bool itemInPoolAreTargeted = false;
    public Animator[] itemPanelAnimators;
    public bool isDoingADraw;

    [ Header("ITEM BUTTON")]
    [SerializeField] private Transform itemButtonsParent;
    [SerializeField] private GameObject itemButton;

    [Header("ITEMS IN GAME")]
    [SerializeField] private List<Item> itemsInGame;
    public List<int> randomIndex = new List<int>();

    [Header("SHUFFLE ANIMATOR")]
    public Animator shuffleAnimator;

    [Header("ITEM PANEL")]
    public ItemPanel[] itemPanels;

    [Header("SFX")]
    [SoundGroup] [SerializeField] private string coinsSFX;

    private CharacterRessources PlayerRessources => Player.GetComponent<CharacterRessources>();
    private EntityStats PlayerStats => Player.GetComponent<EntityStats>();
    public InventoryManager PlayerInventory { get => PlayerRessources.PlayerInventory;  }
    public InventoryBox SelectedInventoryBox { get; set; }
    public bool InventoryItemIsSelected { get => inventoryItemIsSelected; set => inventoryItemIsSelected = value; }
    public bool ShopItemIsSelected { get => shopItemIsSelected; set => shopItemIsSelected = value; }
    public Item SelectedItem { get; set; }
    public List<ShopIcon> ShopBoxesIcon { get => shopBoxesIcon; }
    public int ResetTimes { get; set; }
    public int ResetDrawCost { get; set; }
    public bool ShuffleItemsOnlyWhenFifthWaveIsOver { get => shuffleItemsOnlyWhenFifthWaveIsOver; set => shuffleItemsOnlyWhenFifthWaveIsOver = value; }

    [System.Serializable]
    public class ShopActionData
    {
        public string shopActionDataName;
        public enum ShopActionType { Purchase, Sale }
        public ShopActionType shopActionType;
        public Item item;
        public int itemValue;
        public int transactionID;
        public AbilityEffect abilityEffect;

        public ShopActionData(string shopActionDataName ,ShopActionType shopActionType, Item item = null, int itemValue = 0, int transactionID = 0, AbilityEffect abilityEffect = 0)
        {
            this.shopActionDataName = shopActionDataName;
            this.shopActionType = shopActionType;
            this.item = item;
            this.itemValue = itemValue;
            this.transactionID = transactionID;
            this.abilityEffect = abilityEffect;
        }
    }

    private void Awake ()
    {
        itemsInGame = itemsInGame.OrderBy(itemsInGame => itemsInGame.ItemCost).ToList();
    }

    private void Start()
    {
        if (!costScalesUP)
        {
            ResetDrawCost = costAugmentation;
            OnShopDrawSetResetCost?.Invoke(ResetDrawCost);
        }
    }

    private void OnDisable()
    {
        PlayerRessources.ResetPlayerHUDFeedbackAnimator();
    }

    private void LateUpdate()
    {
        ToggleShuffleAnimatorState();
    }

    #region Buy an item
    public void AddShopActionOnPurchase(InventoryBox inventoryBoxItem = null)
    {
        numberOfShopActionsDone++;
        ResetSelectionIcon();

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        if(inventoryBoxItem != null)
        {
            string shopActionDataName = "Purchase " + inventoryBoxItem.StoredItem.ItemName;

            inventoryBoxItem.StoredItemTransactionID = numberOfShopActionsDone;

            AddShopActionData(shopActionDataName, ShopActionData.ShopActionType.Purchase, null, inventoryBoxItem);

            OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - inventoryBoxItem.StoredItem.ItemCost);
        }
    }
    #endregion

    #region Used on shop buttons
    //Its on a button
    public void BuyItem(Item shopItem)
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase 
            || !CanPurchaseItem(shopItem)
            || PlayerInventory.InventoryIsFull 
            || IsItemAlreadyInInventory(shopItem)) return;

        Debug.Log("Buying item : " + shopItem.ItemName);

        if (UtilityClass.RightClickIsPressed())
        {
            //AddShopActionOnPurchase(null, shopItem);
            PlayerInventory.AddItemToInventory(shopItem, true);
        }
    }

    //Its on a button
    public void BuySelectedItemOnClickingOnButton()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase 
            || !CanPurchaseItem(SelectedItem)
            || PlayerInventory.InventoryIsFull 
            || IsItemAlreadyInInventory(SelectedItem)) return;

        Debug.Log("Buying item : " + SelectedItem.ItemName);
        PlayerInventory.AddItemToInventory(SelectedItem, true);
    }

    //Its on a button
    public void SellItem()
    {
        if (!Player.GetComponent<PlayerController>().IsPlayerInHisBase 
            || PlayerInventory.InventoryIsEmpty) return;

        InventoryBox inventoryBox = null;

        if (SelectedInventoryBox != null) inventoryBox = SelectedInventoryBox;
        else if (SelectedInventoryBox == null)
        {
            for (int i = 0; i < PlayerInventory.InventoryBoxes.Count; i++)
            {
                if (PlayerInventory.InventoryBoxes[i].StoredItem != null)
                {
                    inventoryBox = PlayerInventory.InventoryBoxes[i];
                }
            }
        }

        Debug.Log("Selling item : " + inventoryBox.StoredItem.ItemName);

        ShopActionOnSell(inventoryBox);

        if(SelectedInventoryBox != null) SelectedInventoryBox = null;
    }
    #endregion

    #region Sell an item
    public void ShopActionOnSell(InventoryBox inventoryBoxItem = null)
    {
        string shopActionDataName = "Sale " + inventoryBoxItem.StoredItem.ItemName;

        Debug.Log("Number of shop actions done : " + numberOfShopActionsDone);

        AddShopActionData(shopActionDataName, ShopActionData.ShopActionType.Sale, null, inventoryBoxItem);

        OnSellingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + (inventoryBoxItem.StoredItem.ItemCost / 2));
        ResetSelectionIcon();

        if (inventoryBoxItem != null)
        {
            PlayerInventory.RemoveItemFromInventory(inventoryBoxItem);
            PlayerInventory.ResetAllSelectedIcons();
        }
    }

    private void AddShopActionData(string shopActionDataName, ShopActionData.ShopActionType shopActionType, Item abilityItem = null, InventoryBox inventoryBoxItem = null, AbilityEffect abilityEffect = 0)
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
           shopActionType,
           abilityItem, 
           0, 
           0,
           abilityEffect));
        }

        UtilityClass.PlaySoundGroupImmediatly(coinsSFX, transform);
    }
    #endregion

    #region Undo a purchase or a sell
    //Its on a button
    public void UndoShopAction()
    {
        if (numberOfShopActionsDone > 0)
        {
            UtilityClass.PlaySoundGroupImmediatly(coinsSFX, transform);

            for (int i = shopActions.Count - 1; i >= 0; i--)
            {
                //Undo A Purchase
                if (shopActions[i].shopActionType == ShopActionData.ShopActionType.Purchase)
                {
                    if (shopActions[i].item.ItemIsAnAbility)
                    {
                        shopActions[i].item.UnequipItemAsAbility(
                            shopActions[i].item.AbilityIndex,
                            PlayerStats,
                            shopActions[i].abilityEffect);
                    }
                    else if (!shopActions[i].item.ItemIsAnAbility)
                    {
                        for (int j = 0; j < PlayerInventory.InventoryBoxes.Count; j++)
                        {

                            if (PlayerInventory.InventoryBoxes[j].StoredItemTransactionID == numberOfShopActionsDone)
                            {
                                //Debug.Log(PlayerInventory.InventoryBoxes[j].StoredItemTransactionID + " / " + numberOfShopActionsDone);
                                //Debug.Log(PlayerInventory.InventoryBoxes[j].name);
                                //Debug.Log("Last shop action is a purchase action");

                                PlayerInventory.RemoveItemFromInventory(PlayerInventory.InventoryBoxes[j]);
                                PlayerInventory.ResetAllSelectedIcons();
                                PlayerInventory.InventoryBoxes[j].StoredItemTransactionID = 0;
                            }
                        }
                    }

                    OnShopActionCancel?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources + shopActions[i].item.ItemCost);

                    RefreshShopData();

                    numberOfShopActionsDone--;

                    shopActions.RemoveAt(i);

                    return;
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
                /*&& shopActionData.transactionID == PlayerInventory.InventoryBoxes[i].StoredItemTransactionID*/)
            {
                PlayerInventory.AddItemToInventory(itemToAdd);
                PlayerInventory.InventoryBoxes[i].StoredItemTransactionID = transactionIDData;

                OnShopActionCancel?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - 
                    (itemToAdd.InventoryBox.StoredItem.ItemCost / 2));

                RefreshShopData();

                Debug.Log("Add " + itemToAdd.ItemName + " to inventory");
                Debug.Log("Number of full inventory boxes : " + PlayerInventory.NumberOfFullInventoryBoxes);
                return;
            }
        }
    }
    #endregion

    #region Shuffle
    private void ToggleShuffleAnimatorState()
    {
        if (GameManager.Instance.GameIsInPause() && shuffleAnimator.enabled)
        {
            shuffleAnimator.enabled = false;
        }
        else if (!GameManager.Instance.GameIsInPause() && !shuffleAnimator.enabled)
        {
            shuffleAnimator.enabled = true;
        }
    }

    public void ShuffleFirstAnimation()
    {
        itemInPoolAreTargeted = true;

        OnShuffle?.Invoke();
        DeleteDraw();

        shuffleAnimator.SetTrigger("Shuffle");

        TriggerItemPanelAnimators();
    }

    public void ShuffleAnimation()
    {
        if (ResetDrawCost <= PlayerRessources.CurrentAmountOfPlayerRessources)
        {
            itemInPoolAreTargeted = false;

            OnShuffle?.Invoke();
            DeleteDraw();
            shuffleAnimator.SetTrigger("Shuffle");

            //Reset Shop datas / GO
            if (SelectedItem != null) SelectedItem = null;

            PlayerHUDManager.Instance.ShopInformationPanel.HideContent();

            TriggerItemPanelAnimators();
        } 
    }

    //  -!- It is used by an animator event -!-
    public void ShuffleItemsInShop ()
    {
        //Create the correct amount of item buttons
        for (int i = 0; i < amntOfItemsToAddInThePool; i++)
        {
            GameObject _ItemButton = Instantiate(itemButton);
            _ItemButton.transform.SetParent(itemButtonsParent); 
            _ItemButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            ItemButton itemButtonScript = _ItemButton.GetComponentInChildren<ItemButton>();

            itemCreated.Add(itemButtonScript);
        }

        if (!itemInPoolAreTargeted)
        {
            int randomValue = Random.Range(0, itemsInGame.Count); Debug.Log("Random Value Obtained : " + randomValue);

            //Found different random index until 4 of them have been found
            do
            {
                if (!randomIndex.Contains(randomValue))
                {
                    randomIndex.Add(randomValue);
                }
                else if (randomIndex.Contains(randomValue))
                {
                    randomValue = Random.Range(0, itemsInGame.Count);
                    continue;
                }

            } while (randomIndex.Count != amntOfItemsToAddInThePool);
        }
        else
        {
            int randomValue = Random.Range(0, 18); Debug.Log("Random Value Obtained : " + randomValue);

            //Found different random index until 4 of them have been found
            do
            {
                if (!randomIndex.Contains(randomValue))
                {
                    randomIndex.Add(randomValue);
                }
                else if (randomIndex.Contains(randomValue))
                {
                    randomValue = Random.Range(0, 18);
                    continue;
                }

            } while (randomIndex.Count != amntOfItemsToAddInThePool);
        }
       

        //When all the index have been found => set/populate the item buttons created
        for (int i = randomIndex.Count - 1; i >= 0; i--)
        {
            itemCreated[i].SetButtonInformations(this, itemsInGame[randomIndex[i]]);
            itemCreated[i].ButtonItem.ItemIsInShop = true;

            ShopIcon shopIcon = itemCreated[i].GetComponent<ShopIcon>();
            shopBoxesIcon.Add(shopIcon);

            RefreshShopData();
        }

        Debug.Log("Call OnShuffleDone");
        OnShuffleDone?.Invoke();
    }

    public void DeleteDraw()
    {
        for (int i = itemCreated.Count - 1; i >= 0; i--)
        {
            itemCreated[i].ButtonItem.ItemIsInShop = false;
            Destroy(itemCreated[i].transform.parent.gameObject);
        }

        randomIndex.Clear();
        itemCreated.Clear();
        shopBoxesIcon.Clear();
    }

    //On a button
    public void ResetDrawAndShuffleAgain()
    {
        if (costScalesUP) ResetDrawCost = GetResetDrawCost();
        else ResetDrawCost = costAugmentation;

        if (ResetDrawCost <= PlayerRessources.CurrentAmountOfPlayerRessources)
        {
            ResetTimes++;
            OnShopDrawSetResetCost?.Invoke(ResetDrawCost);
            //DeleteDraw();
            //ShuffleItemsInShop();
            OnBuyingAnItem?.Invoke(PlayerRessources.CurrentAmountOfPlayerRessources - ResetDrawCost);
            RefreshShopData();
        }
    }

    public int GetResetDrawCost()
    {
        return ResetTimes * costAugmentation;
    }
    #endregion

    public void RefreshShopData()
    {
        //Debug.Log("REFRESH SHOP DATA");

        for (int i = 0; i < ShopBoxesIcon.Count; i++)
        {
            if (!ShopBoxesIcon[i].isActiveAndEnabled) continue;

            Item item = ShopBoxesIcon[i].ItemButton.ButtonItem;

            if (!item.ItemIsAnAbility)
            {
                //Debug.Log(item.name + " " + item.ItemIsAnAbility);

                if (/*(*/!IsItemAlreadyInInventory(item)  
                    //&& !canStackSameItem
                    //|| !CanPurchaseItem(item) && canStackSameItem) 
                    && !PlayerInventory.InventoryIsFull)
                {
                    ShopBoxesIcon[i].ItemButton.HidePadlock();
                    ShopBoxesIcon[i].ItemButton.HideCheckMark();

                    if (!CanPurchaseItem(item))
                        ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();

                    else if (CanPurchaseItem(item))
                        ShopBoxesIcon[i].ItemButton.ObjectIsDisponible();
                }
                else if (IsItemAlreadyInInventory(item) /*&& !canStackSameItem*/)
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                    ShopBoxesIcon[i].ItemButton.DisplayCheckMark();
                    ShopBoxesIcon[i].ItemButton.DisplayPadlock();
                }
                else if (PlayerInventory.InventoryIsFull)
                {
                    ShopBoxesIcon[i].ItemButton.ObjectIsNotDisponible();
                    ShopBoxesIcon[i].ItemButton.DisplayPadlock();
                }
            }

            RefreshAllPanelItemsAffordableState();
        }
    }

    private void RefreshAllPanelItemsAffordableState()
    {
        for (int i = 0; i < itemPanels.Length; i++)
        {
            StartCoroutine(itemPanels[i].ToggleEveryNeededCantAffordFeedback());
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
                //Debug.Log(item + " is already in inventory");
                return true;
            }
        }

        return false;
    }

    public bool CanPurchaseItem(Item item)
    {
        bool canPurchaseItem;

        if (item != null && Player.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources >= item.ItemCost)
        {
            canPurchaseItem = true;
        }
        else canPurchaseItem = false;

        return canPurchaseItem;
    }

    public void SetFirstDrawBooleanToTrue()
    {
        firstDrawDone = true;
    }

    private void TriggerItemPanelAnimators()
    {
        foreach (Animator animator in itemPanelAnimators)
        {
            if(!animator.gameObject.activeInHierarchy)
                animator.gameObject.SetActive(true);

            animator.SetTrigger("GoToCenter");
        }
    }

    #region Debuging
    public void ResetShopActions() // Debug purpose
    {
        shopActions.Clear();
        numberOfShopActionsDone = 0;
    }
    #endregion
}