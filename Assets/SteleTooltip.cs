using TMPro;
using UnityEngine;

public class SteleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI effectDescriptionNameText;
    [SerializeField] private TextMeshProUGUI costText;
    private int currentCost;

    private CharacterRessources PlayerRessources => GetComponentInParent<CharacterRessources>();

    public void SetTooltip(string effectName, string effectDescription, string costText, int cost)
    {
        currentCost = cost;

        effectNameText.SetText(effectName);
        effectDescriptionNameText.SetText(effectDescription);

        if (PlayerRessources.CurrentAmountOfPlayerRessources < currentCost) this.costText.color = Color.red;
        else if (PlayerRessources.CurrentAmountOfPlayerRessources >= currentCost) this.costText.color = Color.white;

        this.costText.SetText(costText);
    }

    public void SetCostTextColor()
    {
        if (PlayerRessources.CurrentAmountOfPlayerRessources < currentCost) this.costText.color = Color.red;
        else if (PlayerRessources.CurrentAmountOfPlayerRessources >= currentCost) this.costText.color = Color.white;
    }
}