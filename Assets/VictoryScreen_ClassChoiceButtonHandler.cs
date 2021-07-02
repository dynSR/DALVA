using UnityEngine;
using UnityEngine.EventSystems;

public class VictoryScreen_ClassChoiceButtonHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject highlightObject;
    public bool isSelected = false;
    public VictoryPopupBehaviourHandler victoryPopupBehaviourHandler;

    public void OnPointerClick (PointerEventData eventData)
    {
        if (highlightObject.activeInHierarchy && isSelected)
        {
            highlightObject.SetActive(false);
            isSelected = false;
        }
        else
        {
            victoryPopupBehaviourHandler.ResetAllClassChoiseButtonSelection();

            highlightObject.SetActive(true);
            isSelected = true;
        }
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        if (!isSelected)
        {
            highlightObject.SetActive(true);
        }
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        if (!isSelected)
        {
            highlightObject.SetActive(false);
        }
    }
}
