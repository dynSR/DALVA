using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButtonLogic : MonoBehaviour, IPointerDownHandler
{
    [Header("SHOP BUTTON ATTRIBUTE")]
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private Item shopButtonItem;
    [SerializeField] private Image shopButtonIcon;
    [SerializeField] private TextMeshProUGUI shopButtonItemCostText;

    private ShopIcon ShopIcon => GetComponent<ShopIcon>();

    public Item ShopButtonItem { get => shopButtonItem; }

    void Start() => SetButton();

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on Shop button -!-");
        playerShop.BuyItem(ShopButtonItem);
        ShopIcon.ResetSelection();
    }

    void SetButton()
    {
        shopButtonIcon.sprite = ShopButtonItem.ItemIcon;
        shopButtonItemCostText.text = ShopButtonItem.ItemCost.ToString();
    }
}
