using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerDownHandler
{
    [Header("SHOP BUTTON ATTRIBUTE")]
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI buttonCostText;

    protected ShopIcon ShopIcon => GetComponent<ShopIcon>();
    protected ShopManager PlayerShop { get => playerShop; }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on Shop button -!-");
        //ShopIcon.ResetSelection();
    }

    protected void SetButton(Sprite icon, float cost)
    {
        buttonIcon.sprite = icon;
        buttonCostText.text = cost.ToString("0");
    }
}