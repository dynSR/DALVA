using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButtonLogic : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private Item shopButtonItem;

    private Image ShopButtonIcon => GetComponent<Image>();

    void Start()
    {
        ShopButtonIcon.sprite = shopButtonItem.ItemIcon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        playerShop.BuyItem(shopButtonItem);
    }
}
