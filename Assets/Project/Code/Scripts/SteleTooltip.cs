using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI effectDescriptionNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI titleCostText;
    [SerializeField] private Image effectIconImage;

    private int currentCost;

    private ChangeTextColor changeTextColor;

    private CharacterRessources playerRessources;

    private void OnEnable()
    {
        playerRessources = GameManager.Instance.Player.GetComponent<CharacterRessources>();

        changeTextColor = costText.gameObject.GetComponent<ChangeTextColor>();

        playerRessources.OnCharacterRessourcesChanged += SetCostTextColor;
    }

    private void OnDisable()
    {
        playerRessources.OnCharacterRessourcesChanged -= SetCostTextColor;
    }

    public void SetTooltip(string effectName, string effectDescription, string costText, int cost, bool isASellingButton = false, Sprite effectIconImage = null)
    {
        currentCost = cost;

        effectNameText.SetText(effectName);
        effectDescriptionNameText.SetText(effectDescription);

        if (!isASellingButton)
        {
            titleCostText.gameObject.SetActive(true);

            if (changeTextColor != null)
                changeTextColor.CanChangeTextColor = true;
        }
        else if (isASellingButton)
        {
            titleCostText.gameObject.SetActive(false);

            if (changeTextColor != null)
                changeTextColor.CanChangeTextColor = false;

            if (this.costText.color != Color.white)
            {
                this.costText.color = Color.white;
            }
        }

        if (this.effectIconImage != null)
        {
            this.effectIconImage.sprite = effectIconImage;
        }

        this.costText.SetText(costText);
    }

    public void SetCostTextColor()
    {
        if (!gameObject.activeInHierarchy || changeTextColor != null && !changeTextColor.CanChangeTextColor) return;
        
        if (playerRessources.CurrentAmountOfPlayerRessources < currentCost) costText.color = Color.red;
        else if (playerRessources.CurrentAmountOfPlayerRessources >= currentCost) costText.color = Color.white;
    }
}