using UnityEngine;
using UnityEngine.EventSystems;

public class SteleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,  IButtonTooltip
{
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
        //GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip.GetComponent<SteleTooltip>().SetTooltip();
    }
    public void DisplayTooltip(GameObject tooltip)
    {
        tooltip.SetActive(true);
    }

    public void HideTooltip(GameObject tooltip)
    {
        tooltip.SetActive(false);
    }

    #region Debug
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("COUCOU STELE BUTTON CLIQUÉ");
    }

    public void DebugButton()
    {
        Debug.Log("COUCOU STELE BUTTON CLIQUÉ");
    }
    #endregion
}