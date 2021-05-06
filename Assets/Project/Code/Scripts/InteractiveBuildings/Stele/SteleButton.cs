using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,  IButtonTooltip
{
    [SerializeField] private SteleLogic affectedStele;
    [SerializeField] private SteleLogic.EffectDescription effectDescription;
    [SerializeField] private bool isASellingButton = false;

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

        string steleEffectName = string.Empty;
        string steleEffectDescription = string.Empty;

        switch (affectedStele.SteleLevel)
        {
            case SteleLevel.Default:
                steleEffectName = effectDescription.effectName;
                steleEffectDescription = effectDescription.description;
                break;
            case SteleLevel.EvolutionI:
                if (!isASellingButton)
                {
                    steleEffectName = effectDescription.effectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionI;
                }
                else
                {
                    steleEffectName = effectDescription.effectName;
                    steleEffectDescription = effectDescription.description;
                }
                break;
            case SteleLevel.EvolutionII:
                steleEffectName = effectDescription.effectName;
                steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionII;
                break;
            case SteleLevel.FinalEvolution:
                steleEffectName = effectDescription.effectName;
                steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionFinalEvolution;
                break;
            case SteleLevel.OnlySell:
                steleEffectName = effectDescription.effectName;
                steleEffectDescription = effectDescription.description;
                break;

        }

        DisplayTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
        GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip.GetComponent<SteleTooltip>().SetTooltip(
            /*effectDescription.effectName*/ steleEffectName,
            /*effectDescription.description*/ steleEffectDescription, 
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