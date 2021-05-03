using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,  IButtonTooltip
{
    [SerializeField] private SteleLogic affectedStele;
    [SerializeField] private SteleLogic.EffectDescription effectDescription;

    Button ButtonComponent => GetComponent<Button>();

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
        ButtonComponent.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < effectDescription.effectCost)
            ButtonComponent.interactable = false;

        DisplayTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
        GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip.GetComponent<SteleTooltip>().SetTooltip(
            effectDescription.steleEffect.ToString(), 
            effectDescription.description, 
            effectDescription.effectCost.ToString("0"), 
            effectDescription.effectCost);
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

        if (affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < effectDescription.effectCost)
            ButtonComponent.interactable = false;
        else if (affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources >= effectDescription.effectCost)
            ButtonComponent.interactable = true;
    }

    public void DebugButton()
    {
        Debug.Log("COUCOU STELE BUTTON CLIQUÉ");
    }
    #endregion
}