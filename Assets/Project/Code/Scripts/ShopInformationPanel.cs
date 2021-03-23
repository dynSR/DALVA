using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInformationPanel : MonoBehaviour
{
    [Header("SHOP")]
    [SerializeField] private ShopManager shopManager;

    [Header("BUTTON TO BUY")]
    [SerializeField] private Button buyButton;

    [Header("SELECTED ITEM ATTRIBUTES")]
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemCost;
    [SerializeField] private TextMeshProUGUI selectedItemEffectName;
    [SerializeField] private TextMeshProUGUI selectedItemEffectDescription;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;


    //private void OnEnable()
    //{
    //    ShopIcon.OnSelectingAnItem += SetInformationPanel;
    //}

    //private void OnDisable()
    //{
    //    ShopIcon.OnSelectingAnItem -= SetInformationPanel;
    //}
    
    void SetInformationPanel(Item selectedItem)
    {
        //enable button si le joueur a assez de ressources
        //push toutes les informations de cet équipement
        selectedItemIcon.sprite = selectedItem.ItemIcon;
        selectedItemName.text = selectedItem.ItemName;
        selectedItemCost.text = selectedItem.ItemCost.ToString() ;
        //selectedItemEffectName = selectedItem.ItemEffectName;
        //selectedItemEffectDescription = selectedItem.ItemEffectDescription;
        selectedItemDescription.text = selectedItem.ItemDescription;
    }
}
