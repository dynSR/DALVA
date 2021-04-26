using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : InteractiveButton
{
    [SerializeField] private Item buttonItem;
    public Item ButtonItem { get => buttonItem; }

    void Start() => SetButton(ButtonItem.ItemIcon, ButtonItem.ItemCost);

    public override void OnPointerDown(PointerEventData eventData)
    {
        PlayerShop.BuyItem(ButtonItem);
    }
}