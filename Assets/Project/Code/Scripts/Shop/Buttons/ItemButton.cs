using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : InteractiveButton
{
    [SerializeField] private Item buttonItem;
    public Item ButtonItem { get => buttonItem; private set => buttonItem = value; }

    void Start() => SetButton(ButtonItem.ItemIcon, ButtonItem.ItemCost);

    public override void OnPointerDown(PointerEventData eventData)
    {
        PlayerShop.BuyItem(ButtonItem);
    }

    public void SetButtonInformations(ShopManager shop, Item item)
    {
        PlayerShop = shop;
        ButtonItem = item;
    }
}