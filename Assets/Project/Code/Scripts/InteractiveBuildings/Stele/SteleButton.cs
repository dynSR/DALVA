using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,  IButtonTooltip
{
    [SerializeField] private SteleLogic affectedStele;
    [SerializeField] private SteleLogic.EffectDescription effectDescription;
    [SerializeField] private bool isASellingButton = false;
    [SerializeField] private int finalEvolutionNumber = 0;
    [SerializeField] private Image impossibilityToPurchaseImage;
    [SerializeField] private Image selectionImage;

    Button ButtonComponent => GetComponent<Button>();

    void Awake()
    {
        if(impossibilityToPurchaseImage != null)
            impossibilityToPurchaseImage.gameObject.SetActive(false);

        selectionImage.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if ((!isASellingButton || impossibilityToPurchaseImage != null) && affectedStele.InteractingPlayer != null)
        {
            float playerRessources = affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources;

            if (playerRessources >= effectDescription.effectCost && impossibilityToPurchaseImage.gameObject.activeInHierarchy)
            {
                impossibilityToPurchaseImage.gameObject.SetActive(false);
            }
            else if (playerRessources < effectDescription.effectCost && !impossibilityToPurchaseImage.gameObject.activeInHierarchy)
            {
                impossibilityToPurchaseImage.gameObject.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);
        ButtonComponent.interactable = true;

        if (selectionImage.gameObject.activeInHierarchy)
            selectionImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < effectDescription.effectCost)
            ButtonComponent.interactable = false;

        if (!selectionImage.gameObject.activeInHierarchy)
            selectionImage.gameObject.SetActive(true);

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
                    steleEffectName = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionI;
                }
                else
                {
                    steleEffectName = effectDescription.effectName  + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                }
                break;
            case SteleLevel.EvolutionII:
                if (!isASellingButton)
                {
                    steleEffectName = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionII;
                }
                else
                {
                    steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                }
                break;
            case SteleLevel.FinalEvolution:
                if (!isASellingButton)
                {
                    if (finalEvolutionNumber == 1)
                    {
                        steleEffectName = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                        steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionFinalEvolutionI;
                    }
                    else if (finalEvolutionNumber == 2)
                    {
                        steleEffectName = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                        steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionFinalEvolutionII;
                    }
                }
                else
                {
                    steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                }
                break;
            case SteleLevel.OnlySell:
                steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
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

        selectionImage.gameObject.SetActive(false);
    }

    public void DebugButton()
    {
        Debug.Log("COUCOU STELE BUTTON CLIQUÉ");
    }
    #endregion
}