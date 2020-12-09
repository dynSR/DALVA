using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Shop playerShop;
    [SerializeField] private Item shopButtonItem;

    private Image ShopButtonIcon => GetComponent<Image>();
    private Inventory PlayerInventory => playerShop.PlayerInventory;

    void Start()
    {
        ShopButtonIcon.sprite = shopButtonItem.ItemIcon;
    }

    public void BuyItem(Item shopItem)
    {
        if (!playerShop.Player.GetComponent<PlayerController>().IsPlayerInHisBase) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerInventory.InventoryIsFull) return;

            PlayerInventory.AddPurchasedItemToInventory(shopItem);
            Debug.Log("Buying item : " + shopItem.ItemName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BuyItem(shopButtonItem);
    }
}
