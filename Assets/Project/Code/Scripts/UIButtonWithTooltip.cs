using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonWithTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IButtonTooltip
{
    [SerializeField] private GameObject tooltip;

    public GameObject Tooltip { get => tooltip; }

    protected virtual void OnEnable()
    {
        HideTooltip(Tooltip);
    }

    public void DisplayTooltip(GameObject tooltip)
    {
        if (!tooltip.activeInHierarchy)
            tooltip.SetActive(true);
    }

    public void HideTooltip(GameObject tooltip)
    {
        if (tooltip.activeInHierarchy)
            tooltip.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        DisplayTooltip(Tooltip);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(Tooltip);
    }
}
