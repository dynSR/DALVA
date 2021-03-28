using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerDownHandler
{
    [Header("SHOP BUTTON ATTRIBUTE")]
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private Item buttonItem;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI buttonCostText;

    private ShopIcon ShopIcon => GetComponent<ShopIcon>();

    public Item ButtonItem { get => buttonItem; }

    void Start() => SetButton(ButtonItem.ItemIcon, ButtonItem.ItemCost);

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on Shop button -!-");
        playerShop.BuyItem(ButtonItem);
        ShopIcon.ResetSelection();
    }

    void SetButton(Sprite icon, float cost)
    {
        buttonIcon.sprite = icon;
        buttonCostText.text = cost.ToString("0");
    }
}
