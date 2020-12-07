using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Image ShopButtonIcon => GetComponent<Image>();

    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Item shopButtonItem;

    void Start()
    {
        ShopButtonIcon.sprite = shopButtonItem.ItemIcon;
    }

    public void BuyItem(Item shopButtonItem)
    {
        if (playerInventory.InventoryIsFull) return;

        playerInventory.AddPurchasedItemToInventory(shopButtonItem);
        Debug.Log("Buying item : " + shopButtonItem.ItemName);
    }
}
