using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI effectDescriptionNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI titleCostText;

    private int currentCost;

    private CharacterRessources playerRessources;

    private void OnEnable()
    {
        playerRessources = GameManager.Instance.Player.GetComponent<CharacterRessources>();

        playerRessources.OnCharacterRessourcesChanged += SetCostTextColor;
    }

    private void OnDisable()
    {
        playerRessources.OnCharacterRessourcesChanged -= SetCostTextColor;
    }

    public void SetTooltip(string effectName, string effectDescription, string costText, int cost, bool isASellingButton = false)
    {
        currentCost = cost;

        effectNameText.SetText(effectName);
        effectDescriptionNameText.SetText(effectDescription);

        if (!isASellingButton)
        {
            SetCostTextColor();
            titleCostText.gameObject.SetActive(true);
        }
        else titleCostText.gameObject.SetActive(false);

        this.costText.SetText(costText);
    }

    public void SetCostTextColor()
    {
        if (!gameObject.activeInHierarchy) return;
        
        if (playerRessources.CurrentAmountOfPlayerRessources < currentCost) this.costText.color = Color.red;
        else if (playerRessources.CurrentAmountOfPlayerRessources >= currentCost) this.costText.color = Color.white;
    }
}