using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class InteractiveButton : MonoBehaviour, IPointerDownHandler
{
    [Header("SHOP BUTTON ATTRIBUTE")]
    [SerializeField] private Image buttonIcon;
    [SerializeField] private TextMeshProUGUI buttonCostText;

    [Header("DISPONIBILITY ATTRIBUTES")]
    [SerializeField] private GameObject padlockImage;
    [SerializeField] private Image checkMark;
    [SerializeField] private GameObject undisponibilityGameObject;

    protected ShopIcon ShopIcon => GetComponent<ShopIcon>();
    protected ShopManager PlayerShop { get; set; }

    public abstract void OnPointerDown(PointerEventData eventData);

    protected void SetButton(Sprite icon, float cost)
    {
        buttonIcon.sprite = icon;
        buttonCostText.text = cost.ToString("0");
    }

    public void ObjectIsNotDisponible()
    {
        Debug.Log("ObjectIsNotDisponible");
        undisponibilityGameObject.SetActive(true);
    }

    public void ObjectIsDisponible()
    {
        Debug.Log("ObjectIsDisponible");
        undisponibilityGameObject.SetActive(false);
    }

    public void DisplayPadlock()
    {
        Debug.Log("DisplayPadlock");
        padlockImage.SetActive(true);
    }
    public void HidePadlock()
    {
        Debug.Log("HidePadlock");
        padlockImage.SetActive(false);
    }

    public void DisplayCheckMark()
    {
        Debug.Log("DisplayCheckMark");
        checkMark.enabled = true;
    }

    public void HideCheckMark()
    {
        Debug.Log("HideCheckMark");
        checkMark.enabled = false;
    }
}