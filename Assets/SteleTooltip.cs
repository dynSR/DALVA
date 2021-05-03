using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SteleTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI effectDescriptionNameText;
    [SerializeField] private TextMeshProUGUI costText;

    private CharacterRessources PlayerRessources => GetComponentInParent<CharacterRessources>();

    public void SetTooltip(string effectName, string effectDescription, string costText, int cost)
    {
        effectNameText.SetText(effectName);
        effectDescriptionNameText.SetText(effectDescription);

        if (PlayerRessources.CurrentAmountOfPlayerRessources < cost) this.costText.color = Color.red;
        else if (PlayerRessources.CurrentAmountOfPlayerRessources >= cost) this.costText.color = Color.white;
        this.costText.SetText(costText);
    }
}