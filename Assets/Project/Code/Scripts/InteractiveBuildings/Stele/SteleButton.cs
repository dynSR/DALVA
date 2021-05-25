using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteleButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,  IPointerUpHandler, IButtonTooltip
{
    [SerializeField] private GameObject buttonTooltip;
    [SerializeField] private SteleLogic affectedStele;
    [SerializeField] private SteleLogic.EffectDescription effectDescription;
    [SerializeField] private bool isASellingButton = false;
    [SerializeField] private Image impossibilityToPurchaseImage;
    [SerializeField] private Image selectionImage;

    Button ButtonComponent => GetComponent<Button>();

    public bool IsASellingButton { get => isASellingButton; set => isASellingButton = value; }

    void Awake()
    {
        if(impossibilityToPurchaseImage != null)
            impossibilityToPurchaseImage.gameObject.SetActive(false);

        selectionImage.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if ((!IsASellingButton || impossibilityToPurchaseImage != null) && affectedStele.InteractingPlayer != null)
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
        HideTooltip(buttonTooltip);
        ButtonComponent.interactable = true;

        if (selectionImage.gameObject.activeInHierarchy)
            selectionImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsASellingButton && affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < effectDescription.effectCost)
            ButtonComponent.interactable = false;

        if (!selectionImage.gameObject.activeInHierarchy)
            selectionImage.gameObject.SetActive(true);

        string steleEffectName = string.Empty;
        string steleEffectDescription = string.Empty;
        Sprite effectIcon = null;

        switch (affectedStele.SteleLevel)
        {
            case SteleLevel.Default:
                steleEffectName = effectDescription.effectName;
                steleEffectDescription = effectDescription.description;
                effectIcon = effectDescription.effectIcon;
                break;
            case SteleLevel.EvolutionI:
                if (!IsASellingButton)
                {
                    steleEffectName = "Amélioration I" + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionI;
                    effectIcon = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleIconImage;
                }
                else
                {
                    steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                    effectIcon = effectDescription.effectIcon;
                }
                break;
            case SteleLevel.EvolutionII:
                if (!IsASellingButton)
                {
                    steleEffectName = "Amélioration II" + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionII;
                    effectIcon = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleIconImage;
                }
                else
                {
                    steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                    effectIcon = effectDescription.effectIcon;
                }
                break;
            case SteleLevel.FinalEvolution:
                if (!IsASellingButton)
                {
                    effectIcon = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleIconImage;

                    steleEffectName = "Amélioration III" + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeDescriptionFinalEvolution;
                }
                else
                {
                    steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                    steleEffectDescription = effectDescription.description;
                    effectIcon = effectDescription.effectIcon;
                }
                break;
            case SteleLevel.OnlySell:
                steleEffectName = effectDescription.effectName + '\n' + affectedStele.SpawnedEffectObject.GetComponent<SteleAmelioration>().SteleEffectName;
                steleEffectDescription = effectDescription.description;
                effectIcon = effectDescription.effectIcon;
                break;

        }

        #region Player HUD Tooltip
        //Tooltip en bas à droite
        DisplayTooltip(GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip);

        SteleTooltip steleTooltip = GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip.GetComponent<SteleTooltip>();

        steleTooltip.SetTooltip(
            /*effectDescription.effectName*/ steleEffectName,
            /*effectDescription.description*/ steleEffectDescription, 
            effectDescription.effectCost.ToString("0"), 
            effectDescription.effectCost, IsASellingButton, effectIcon);

        steleTooltip.SetCostTextColor();
        #endregion

        #region Button Tooltip
        //Tooltip au-dessus des boutons
        DisplayTooltip(buttonTooltip);

        buttonTooltip.GetComponent<SteleTooltip>().SetTooltip(
            /*effectDescription.effectName*/ steleEffectName,
            /*effectDescription.description*/ steleEffectDescription,
            effectDescription.effectCost.ToString("0"),
            effectDescription.effectCost, IsASellingButton, effectIcon);

        steleTooltip.SetCostTextColor();
        #endregion
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

        if (!IsASellingButton && affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < effectDescription.effectCost)
            ButtonComponent.interactable = false;
        else if (!IsASellingButton && affectedStele.InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources >= effectDescription.effectCost)
            ButtonComponent.interactable = true;

        selectionImage.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HideTooltip(buttonTooltip);
    }

    public void DebugButton()
    {
        Debug.Log("COUCOU STELE BUTTON CLIQUÉ");
    }
    #endregion
}