using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    public Image iconItemIsInShop;

    public TooltipSetter tooltipSetter = null;

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

    public void DisplayItemIsntInShop()
    {
        iconItemIsInShop.gameObject.SetActive(true);
    }

    public void HideThatItemIsInShop()
    {
        iconItemIsInShop.gameObject.SetActive(false);
    }

    public void DisableCanvasGroup()
    {
        CanvasGroup cG = GetComponent<CanvasGroup>();

        if (cG != null)
        {
            cG.alpha = 0;
            cG.blocksRaycasts = false;
        }
    }

    public void EnableCanvasGroup()
    {
        CanvasGroup cG = GetComponent<CanvasGroup>();

        if (cG != null)
        {
            cG.alpha = 1;
            cG.blocksRaycasts = true;
        }
    }
}
