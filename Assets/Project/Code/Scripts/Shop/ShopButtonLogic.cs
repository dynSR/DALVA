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

    void Start() => SetButton();

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on Shop button -!-");
        playerShop.BuyItem(shopButtonItem);
    }

    void SetButton()
    {
        shopButtonIcon.sprite = shopButtonItem.ItemIcon;
        shopButtonItemCostText.text = shopButtonItem.ItemCost.ToString();
    }
}
