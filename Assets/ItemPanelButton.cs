using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemPanel ItemPanelParent { get; set; }
    public Image buttonImage;
    public Image iconItemIsInShop;
    public GameObject rotatingFeedback;

    public TooltipSetter tooltipSetter = null;
    public GameObject cantAffordFeedbackObject;

    public Item AttributedItem { get; set; }

    #region Tooltip handle
    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayTooltip();
        tooltipSetter.SetTooltip(AttributedItem.ItemName, AttributedItem.ItemDescription, AttributedItem.ItemCost.ToString("0"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    public void HideTooltip()
    {
        if (tooltipSetter.gameObject.activeInHierarchy)
            tooltipSetter.gameObject.SetActive(false);
    }

    public void DisplayTooltip()
    {
        if (!tooltipSetter.gameObject.activeInHierarchy)
            tooltipSetter.gameObject.SetActive(true);
    }
    #endregion

    private void Start ()
    {
        ToggleCantAffordFeedback();
    }

    public void ItemIsntInShop()
    {
        iconItemIsInShop.gameObject.SetActive(true);
        rotatingFeedback.SetActive(false);
    }

    public void ItemIsInShop()
    {
        iconItemIsInShop.gameObject.SetActive(false);
        rotatingFeedback.SetActive(true);
    }

    public void DisableCanvasGroup()
    {
        Debug.Log("DisableCanvasGroup");

        CanvasGroup cG = GetComponent<CanvasGroup>();

        if (cG != null)
        {
            cG.alpha = 0;
            cG.blocksRaycasts = false;
        }
    }

    public void EnableCanvasGroup()
    {
        Debug.Log("EnableCanvasGroup");

        CanvasGroup cG = GetComponent<CanvasGroup>();

        if (cG != null)
        {
            cG.alpha = 1;
            cG.blocksRaycasts = true;
        }
    }

    public void ToggleCantAffordFeedback ()
    {
        if (ItemPanelParent.shopManager.CanPurchaseItem(AttributedItem))
        {
            cantAffordFeedbackObject.SetActive(false);
        }
        else cantAffordFeedbackObject.SetActive(true);
    }
}
